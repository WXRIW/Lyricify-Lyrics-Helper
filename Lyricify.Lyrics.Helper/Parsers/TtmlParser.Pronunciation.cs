using Lyricify.Lyrics.Models;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Lyricify.Lyrics.Parsers
{
    public static partial class TtmlParser
    {
        private sealed class PronunciationLine
        {
            public XElement Element { get; init; } = null!;
            public ILineInfo Line { get; set; } = null!;
            public int Index { get; init; }
            public List<SyllableInfo> BackgroundSyllables { get; init; } = new();
            public List<SourceBackground> BackgroundGroups { get; init; } = new();
        }

        private sealed class SourceBackground
        {
            public XElement Element { get; init; } = null!;
            public List<SyllableInfo> Syllables { get; } = new();
        }

        private sealed class PronunciationPart
        {
            public string Text { get; set; } = "";
            public int? Begin { get; init; }
            public int? End { get; init; }
            public XElement? ClockOwner { get; init; }
            public bool IsWhitespaceGap { get; init; }
        }

        private sealed class PronunciationStream
        {
            public XElement? Element { get; init; }
            public List<PronunciationPart> Parts { get; } = new();
            public bool HasContent { get; set; }
        }

        /// <summary>
        /// Imports native Apple Music TTML transliterations into the shared lyric models
        /// and associates them with the original lyric units. Text splitting remains in PrepareLyrics.
        /// </summary>
        private static LyricsData ParsePronunciationDocument(string ttml,
            bool useEmbeddedSimplifiedChineseLyrics, Action<string>? diagnostic)
        {
            if (string.IsNullOrWhiteSpace(ttml))
                return ParseCore(ttml, useEmbeddedSimplifiedChineseLyrics);

            XDocument document;
            try
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    MaxCharactersInDocument = 4 * 1024 * 1024,
                    MaxCharactersFromEntities = 0
                };
                using var input = new System.IO.StringReader(ttml);
                using var reader = XmlReader.Create(input, settings);
                document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
                var pending = new Stack<(XNode node, int depth)>();
                if (document.Root != null) pending.Push((document.Root, 0));
                var count = 0;
                while (pending.Count > 0)
                {
                    var (node, depth) = pending.Pop();
                    if (++count > 200000 || depth > 128)
                        throw new XmlException("TTML exceeds the supported node or nesting limit.");
                    if (node is XElement element)
                        foreach (var child in element.Nodes()) pending.Push((child, depth + 1));
                }
            }
            catch (Exception exception) when (exception is XmlException || exception is ArgumentException)
            {
                diagnostic?.Invoke("TTML pronunciation parsing failed: " + exception.Message);
                return ParseCore("", useEmbeddedSimplifiedChineseLyrics);
            }

            if (document.Root?.Name != NsTtml + "tt")
            {
                diagnostic?.Invoke("Pronunciation metadata was skipped because the root is not a TTML tt element.");
                return ParseCore(ttml, useEmbeddedSimplifiedChineseLyrics, document);
            }

            var lines = new List<PronunciationLine>();
            var data = ParseCore(ttml, useEmbeddedSimplifiedChineseLyrics, document, lines);
            AttachPronunciations(document, data, lines, diagnostic);
            return data;
        }

        private static void AttachPronunciations(XDocument document, LyricsData data,
            List<PronunciationLine> lines, Action<string>? diagnostic)
        {
            var keyedLines = lines.Where(line => !string.IsNullOrEmpty(GetLineKey(line.Element)))
                .GroupBy(line => GetLineKey(line.Element)!, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
            var languageOrder = new List<string>();
            var metadata = document.Root!.Element(NsTtml + "head")?.Element(NsTtml + "metadata");
            if (metadata == null) return;

            var containers = metadata.Elements().SelectMany(element =>
                element.Name == NsItunes + "transliterations" ? new[] { element }
                : element.Name == NsItunes + "iTunesMetadata" ? element.Elements(NsItunes + "transliterations")
                : Enumerable.Empty<XElement>()).ToList();
            var duplicateEntries = containers.SelectMany(container => container.Elements(NsItunes + "transliteration"))
                .SelectMany(track => track.Elements(NsItunes + "text"))
                .Where(text => text.Attribute("for") != null)
                .GroupBy(text => ((string)text.Attribute("for")!, InheritedLanguage(text).ToUpperInvariant()))
                .Where(group => group.Count() > 1).Select(group => group.Key).ToHashSet();
            var duplicateLineKeys = document.Descendants(NsTtml + "p")
                .Where(element => GetLineKey(element) != null)
                .GroupBy(element => GetLineKey(element)!, StringComparer.Ordinal)
                .Where(group => group.Count() > 1).Select(group => group.Key).ToHashSet(StringComparer.Ordinal);
            foreach (var container in containers)
            {
                foreach (var track in container.Elements())
                {
                    if (track.Name != NsItunes + "transliteration")
                    {
                        diagnostic?.Invoke("Unsupported pronunciation track: " + track.Name + ".");
                        continue;
                    }
                    var language = InheritedLanguage(track);
                    if (!languageOrder.Contains(language, StringComparer.OrdinalIgnoreCase))
                        languageOrder.Add(language);
                    foreach (var text in track.Elements())
                    {
                        if (text.Name != NsItunes + "text")
                        {
                            diagnostic?.Invoke("Unsupported pronunciation entry: " + text.Name + ".");
                            continue;
                        }
                        var key = (string?)text.Attribute("for");
                        if (string.IsNullOrEmpty(key) || !keyedLines.TryGetValue(key, out var matches))
                        {
                            diagnostic?.Invoke("Unknown pronunciation line key: " + (key ?? "<missing>") + ".");
                            continue;
                        }
                        if (matches.Count != 1 || duplicateLineKeys.Contains(key))
                        {
                            diagnostic?.Invoke("Ambiguous pronunciation line key: " + key + ".");
                            continue;
                        }
                        var entryLanguage = InheritedLanguage(text);
                        if (duplicateEntries.Contains((key, entryLanguage.ToUpperInvariant())))
                        {
                            diagnostic?.Invoke("Duplicate pronunciation entry for " + key + " / " + entryLanguage + ".");
                            continue;
                        }
                        if (!languageOrder.Contains(entryLanguage, StringComparer.OrdinalIgnoreCase))
                            languageOrder.Add(entryLanguage);

                        var line = matches[0];
                        var streams = ReadPronunciationStreams(text, diagnostic);
                        if (streams[0].HasContent || streams.Count == 1)
                        {
                            line.Line = EnsureFullLine(line.Line);
                            data.Lines![line.Index] = line.Line;
                            AssociateStream(line.Line, GetSyllables(line.Line), streams[0], entryLanguage, key, diagnostic);
                        }
                        AttachBackgroundPronunciations(line, streams.Skip(1).ToList(), entryLanguage, key, diagnostic);
                    }
                }
            }

            foreach (var line in lines)
            {
                SetDefaultPronunciationLanguage(line.Line, InheritedLanguage(line.Element), languageOrder);
                if (line.Line.SubLine != null)
                    SetDefaultPronunciationLanguage(line.Line.SubLine, InheritedLanguage(line.Element), languageOrder);
            }
        }

        private static string? GetLineKey(XElement element)
            => (string?)element.Attribute(NsItunes + "key") ?? (string?)element.Attribute("key");

        private static string InheritedLanguage(XElement element)
            => element.AncestorsAndSelf().Select(ancestor => ancestor.Attribute(NsXml + "lang"))
                .FirstOrDefault(attribute => attribute != null)?.Value.Trim() ?? "";

        private static bool IsBackgroundSpan(XElement element)
            => (element.Name == NsTtml + "span" || element.Name == NsItunes + "span")
                && string.Equals((string?)element.Attribute(NsTtm + "role") ?? (string?)element.Attribute("role"),
                    "x-bg", StringComparison.OrdinalIgnoreCase);

        private static ILineInfo EnsureFullLine(ILineInfo line)
            => line is FullLineInfo || line is FullSyllableLineInfo ? line
                : line is SyllableLineInfo syllableLine ? new FullSyllableLineInfo(syllableLine)
                : line is LineInfo plainLine ? new FullLineInfo(plainLine) : line;

        private static List<SyllableInfo> GetSyllables(ILineInfo line)
            => line is SyllableLineInfo syllableLine ? syllableLine.Syllables.SelectMany(item =>
                item is SyllableInfo syllable ? new[] { syllable }
                : item is FullSyllableInfo group ? group.SubItems : Enumerable.Empty<SyllableInfo>()).ToList()
                : new List<SyllableInfo>();

        private static void AssociateStream(ILineInfo line, IReadOnlyList<SyllableInfo> syllables,
            PronunciationStream stream, string language, string key, Action<string>? diagnostic)
        {
            var parts = stream.Parts.Count == 0
                ? new List<PronunciationPart> { new() }
                : stream.Parts;
            if (TryPlanAssociation(line, syllables, stream, out var associations))
            {
                foreach (var (part, first, last) in associations)
                {
                    var pronunciations = first == last ? syllables[first].Pronunciations
                        : GetOrCreatePronunciationGroup((SyllableLineInfo)line, syllables, first, last)
                            .GroupPronunciations;
                    if (!pronunciations.TryGetValue(language, out var values))
                        pronunciations[language] = values = new List<string>();
                    values.Add(part.Text);
                }
            }
            else
            {
                var values = parts.Select(part => part.Text).ToList();
                if (line is FullSyllableLineInfo syllableLine)
                    syllableLine.UnassignedPronunciations[language] = values;
                else if (line is FullLineInfo fullLine)
                    fullLine.Pronunciations[language] = values;
                if (syllables.Count > 1)
                    diagnostic?.Invoke("Pronunciation for " + key + " / " + language
                        + " could not be associated uniquely in source order; the complete line was retained.");
            }
        }

        private static void AttachBackgroundPronunciations(PronunciationLine line,
            List<PronunciationStream> streams, string language, string key, Action<string>? diagnostic)
        {
            if (streams.Count == 0) return;
            if (line.Line.SubLine == null)
            {
                diagnostic?.Invoke("Pronunciation background has no source background for " + key + ".");
                return;
            }
            var subLine = EnsureFullLine(line.Line.SubLine);
            SetSubLine(line.Line, subLine);
            var finalSyllables = GetSyllables(subLine);
            var mapped = new List<(PronunciationStream stream, List<SyllableInfo> syllables)>();
            var used = new HashSet<SourceBackground>();
            var ambiguous = false;
            foreach (var stream in streams)
            {
                var identity = (string?)stream.Element?.Attribute("for")
                    ?? (string?)stream.Element?.Attribute(NsItunes + "key")
                    ?? (string?)stream.Element?.Attribute(NsXml + "id");
                SourceBackground? group = null;
                if (!string.IsNullOrEmpty(identity))
                {
                    var matches = line.BackgroundGroups.Where(candidate =>
                        string.Equals(identity, (string?)candidate.Element.Attribute(NsItunes + "key")
                            ?? (string?)candidate.Element.Attribute(NsXml + "id"), StringComparison.Ordinal)).ToList();
                    if (matches.Count == 1) group = matches[0];
                }
                else if (streams.Count == line.BackgroundGroups.Count)
                    group = line.BackgroundGroups[mapped.Count];

                if (group == null || !used.Add(group))
                {
                    ambiguous = true;
                    mapped.Add((stream, new List<SyllableInfo>()));
                    continue;
                }
                var targetSyllables = new List<SyllableInfo>();
                if (finalSyllables.Count == line.BackgroundSyllables.Count)
                {
                    foreach (var source in group.Syllables)
                    {
                        var index = line.BackgroundSyllables.IndexOf(source);
                        if (index >= 0) targetSyllables.Add(finalSyllables[index]);
                    }
                }
                else if (line.BackgroundGroups.Count == 1)
                    targetSyllables = finalSyllables;
                if (targetSyllables.Count == 0) ambiguous = true;
                mapped.Add((stream, targetSyllables));
            }

            // A single fallback contains the whole background language. This avoids
            // combining an incomplete stream with already attached children twice.
            if (ambiguous || mapped.Any(item => !TryPlanAssociation(subLine, item.syllables, item.stream, out _)))
            {
                var whole = new PronunciationStream { HasContent = true };
                foreach (var stream in streams) whole.Parts.AddRange(stream.Parts);
                AssociateStream(subLine, new List<SyllableInfo>(), whole, language, key + " background", diagnostic);
                diagnostic?.Invoke("Pronunciation background groups for " + key
                    + " could not be associated uniquely; complete background text was retained.");
                return;
            }
            foreach (var (stream, syllables) in mapped)
                AssociateStream(subLine, syllables, stream, language, key + " background", diagnostic);
        }

        private static bool TryPlanAssociation(ILineInfo line, IReadOnlyList<SyllableInfo> syllables,
            PronunciationStream stream,
            out List<(PronunciationPart part, int first, int last)> associations)
        {
            associations = new();
            if (syllables.Count == 0) return false;
            var parts = stream.Parts.Count == 0
                ? new List<PronunciationPart> { new() } : stream.Parts;
            if (syllables.Count > 1 && stream.Parts.Count == 0) return false;
            var previousFirst = -1;
            var previousLast = -1;
            foreach (var part in parts)
            {
                var (first, last) = FindPronunciationTarget(syllables, part);
                // Identical targets may receive several parts. Otherwise ranges must
                // advance without overlap so grouping cannot reorder the native text.
                if (first < 0 || (first <= previousLast
                    && (first != previousFirst || last != previousLast))) return false;
                if (first != last && (line is not SyllableLineInfo syllableLine
                    || !CanRepresentPronunciationGroup(syllableLine, syllables, first, last))) return false;
                associations.Add((part, first, last));
                previousFirst = first;
                previousLast = last;
            }
            return true;
        }

        private static (int first, int last) FindPronunciationTarget(
            IReadOnlyList<SyllableInfo> syllables, PronunciationPart part)
        {
            if (syllables.Count == 1) return (0, 0);
            if (!part.Begin.HasValue || !part.End.HasValue || part.End < part.Begin) return (-1, -1);
            var target = -1;
            for (var index = 0; index < syllables.Count; index++)
            {
                if (part.Begin < syllables[index].StartTime || part.End > syllables[index].EndTime) continue;
                if (target >= 0) return (-1, -1);
                target = index;
            }
            if (target >= 0) return (target, target);

            // A shared pronunciation belongs to an explicit group only when its
            // complete interval exactly identifies adjacent, non-overlapping units.
            // Partial coverage or overlapping source clocks do not establish a group.
            var match = (first: -1, last: -1);
            for (var first = 0; first < syllables.Count - 1; first++)
            {
                if (syllables[first].StartTime != part.Begin) continue;
                var ordered = syllables[first].EndTime >= syllables[first].StartTime;
                for (var last = first + 1; last < syllables.Count; last++)
                {
                    ordered &= syllables[last].StartTime >= syllables[last - 1].EndTime
                        && syllables[last].EndTime >= syllables[last].StartTime;
                    if (!ordered) break;
                    if (syllables[last].EndTime != part.End) continue;
                    if (Enumerable.Range(0, syllables.Count).Any(index => (index < first || index > last)
                        && syllables[index].StartTime < part.End && syllables[index].EndTime > part.Begin)) continue;
                    if (match.first >= 0) return (-1, -1);
                    match = (first, last);
                }
            }
            return match;
        }

        private static bool CanRepresentPronunciationGroup(SyllableLineInfo line,
            IReadOnlyList<SyllableInfo> syllables, int first, int last)
        {
            var children = syllables.Skip(first).Take(last - first + 1).ToList();
            if (line.Syllables.OfType<FullSyllableInfo>().Any(group => group.SubItems.SequenceEqual(children)))
                return true;
            var start = line.Syllables.FindIndex(item => ReferenceEquals(item, children[0]));
            return start >= 0 && start + children.Count <= line.Syllables.Count
                && line.Syllables.Skip(start).Take(children.Count).SequenceEqual(children);
        }

        private static FullSyllableInfo GetOrCreatePronunciationGroup(SyllableLineInfo line,
            IReadOnlyList<SyllableInfo> syllables, int first, int last)
        {
            var children = syllables.Skip(first).Take(last - first + 1).ToList();
            var existing = line.Syllables.OfType<FullSyllableInfo>()
                .FirstOrDefault(group => group.SubItems.SequenceEqual(children));
            if (existing != null) return existing;
            var start = line.Syllables.FindIndex(item => ReferenceEquals(item, children[0]));
            var group = new FullSyllableInfo(children);
            line.Syllables.RemoveRange(start, children.Count);
            line.Syllables.Insert(start, group);
            line.RefreshProperties();
            return group;
        }

        private static void SetDefaultPronunciationLanguage(ILineInfo line, string sourceLanguage,
            IReadOnlyList<string> languageOrder)
        {
            IEnumerable<string> available = line is FullSyllableLineInfo syllableLine ? syllableLine.Pronunciations.Keys
                : line is FullLineInfo fullLine ? fullLine.Pronunciations.Keys : Array.Empty<string>();
            var languages = available.ToList();
            var primary = sourceLanguage.Split('-')[0];
            var matching = languages.Where(language => primary.Length > 0
                && string.Equals(language.Split('-')[0], primary, StringComparison.OrdinalIgnoreCase)).ToList();
            var candidates = matching.Count > 0 ? matching : languages;
            var selected = candidates.OrderByDescending(language => language.Split('-')
                    .Any(part => part.Equals("Latn", StringComparison.OrdinalIgnoreCase)))
                .ThenBy(language => FindLanguageIndex(languageOrder, language)).FirstOrDefault();
            if (line is FullSyllableLineInfo fullSyllableLine) fullSyllableLine.PronunciationLanguage = selected;
            else if (line is FullLineInfo full) full.PronunciationLanguage = selected;
        }

        private static int FindLanguageIndex(IReadOnlyList<string> languages, string language)
        {
            for (var i = 0; i < languages.Count; i++)
                if (languages[i].Equals(language, StringComparison.OrdinalIgnoreCase)) return i;
            return int.MaxValue;
        }

        private static List<PronunciationStream> ReadPronunciationStreams(XElement text, Action<string>? diagnostic)
        {
            var streams = new List<PronunciationStream> { new() };
            var unsupportedTiming = text.AncestorsAndSelf().Any(element =>
                element.Attribute("timeContainer") is XAttribute container
                && !container.Value.Equals("par", StringComparison.OrdinalIgnoreCase));
            if (unsupportedTiming)
                diagnostic?.Invoke("Unsupported pronunciation time container; its text was retained.");
            ReadPronunciationNodes(text.Nodes(), streams[0], streams, null, null, null,
                false, unsupportedTiming, diagnostic);
            foreach (var stream in streams)
            {
                // Inter-span whitespace belongs to the preceding pronunciation;
                // leading whitespace belongs to the following one. Neither adds a unit.
                for (var i = stream.Parts.Count - 1; i >= 0; i--)
                {
                    var part = stream.Parts[i];
                    if (!part.IsWhitespaceGap) continue;
                    if (i > 0)
                    {
                        stream.Parts[i - 1].Text += part.Text;
                        stream.Parts.RemoveAt(i);
                    }
                    else if (stream.Parts.Count > 1)
                    {
                        stream.Parts[1].Text = part.Text + stream.Parts[1].Text;
                        stream.Parts.RemoveAt(i);
                    }
                }
            }
            return streams;
        }

        private static void ReadPronunciationNodes(IEnumerable<XNode> nodes, PronunciationStream stream,
            List<PronunciationStream> streams, int? begin, int? end, XElement? clockOwner,
            bool background, bool unsupportedTiming, Action<string>? diagnostic)
        {
            foreach (var node in nodes)
            {
                if (node is XText value)
                {
                    stream.HasContent = true;
                    AddPronunciationPart(stream, value.Value, begin, end, clockOwner,
                        string.IsNullOrWhiteSpace(value.Value) && value.Parent?.Elements().Any() == true);
                    continue;
                }
                if (node is not XElement element) continue;
                var recognized = element.Name.Namespace == NsTtml || element.Name.Namespace == NsItunes;
                if (recognized && element.Name.LocalName == "br")
                {
                    stream.HasContent = true;
                    AddPronunciationPart(stream, "\n", begin, end, clockOwner);
                    continue;
                }
                var isSpan = recognized && element.Name.LocalName == "span";
                var target = stream;
                var isBackground = background || IsBackgroundSpan(element);
                if (!background && isBackground)
                {
                    target = new PronunciationStream { Element = element, HasContent = true };
                    streams.Add(target);
                }
                var nextBegin = begin;
                var nextEnd = end;
                var nextOwner = clockOwner;
                var nextUnsupportedTiming = unsupportedTiming;
                if (isSpan)
                {
                    // Preserve an explicit outer span as one pronunciation unit,
                    // including when its original lyric unit needs no timing to match.
                    nextOwner ??= element;
                    if (element.Attribute("begin") != null || element.Attribute("end") != null)
                    {
                        nextOwner = element;
                        if (element.Attribute("begin") is XAttribute beginAttribute)
                            nextBegin = ParsePronunciationTime(beginAttribute.Value);
                        if (element.Attribute("end") is XAttribute endAttribute)
                            nextEnd = ParsePronunciationTime(endAttribute.Value);
                        if (!nextBegin.HasValue || !nextEnd.HasValue || nextEnd < nextBegin)
                            diagnostic?.Invoke("Invalid or incomplete pronunciation interval; its text was retained.");
                    }
                    if (element.Attribute("timeContainer") is XAttribute container
                        && !container.Value.Equals("par", StringComparison.OrdinalIgnoreCase))
                    {
                        nextUnsupportedTiming = true;
                        nextOwner = element;
                        diagnostic?.Invoke("Unsupported pronunciation time container; its text was retained.");
                    }
                }
                else
                    diagnostic?.Invoke("Unsupported pronunciation wrapper: " + element.Name + ".");
                if (nextUnsupportedTiming) nextBegin = nextEnd = null;
                if (!element.Nodes().Any())
                {
                    target.HasContent = true;
                    AddPronunciationPart(target, "", nextBegin, nextEnd, nextOwner);
                }
                else
                    ReadPronunciationNodes(element.Nodes(), target, streams, nextBegin, nextEnd,
                        nextOwner, isBackground, nextUnsupportedTiming, diagnostic);
                if (!isBackground) stream.HasContent = true;
            }
        }

        private static void AddPronunciationPart(PronunciationStream stream, string text,
            int? begin, int? end, XElement? clockOwner, bool isWhitespaceGap = false)
        {
            if (stream.Parts.Count > 0 && stream.Parts[^1].ClockOwner == clockOwner
                && stream.Parts[^1].Begin == begin && stream.Parts[^1].End == end
                && stream.Parts[^1].IsWhitespaceGap == isWhitespaceGap)
                stream.Parts[^1].Text += text;
            else
                stream.Parts.Add(new PronunciationPart
                {
                    Text = text, Begin = begin, End = end, ClockOwner = clockOwner,
                    IsWhitespaceGap = isWhitespaceGap
                });
        }

        private static int? ParseSourcePronunciationTime(string? value)
            => value == null ? null : ParsePronunciationTime(value);

        private static int? ParsePronunciationTime(string value)
        {
            value = value.Trim();
            if (value.Length == 0) return null;
            double milliseconds;
            if (value.EndsWith("ms", StringComparison.OrdinalIgnoreCase))
            {
                if (!TryPronunciationNumber(value[..^2], out milliseconds)) return null;
            }
            else
            {
                if (value.EndsWith("s", StringComparison.OrdinalIgnoreCase)) value = value[..^1];
                var parts = value.Split(':');
                if (parts.Length < 1 || parts.Length > 3) return null;
                double seconds = 0;
                for (var i = 0; i < parts.Length; i++)
                {
                    if (!TryPronunciationNumber(parts[i], out var component)
                        || (parts.Length > 1 && i > 0 && component >= 60)
                        || (i < parts.Length - 1 && component != Math.Truncate(component))) return null;
                    seconds = seconds * 60 + component;
                }
                milliseconds = seconds * 1000;
            }
            if (double.IsNaN(milliseconds) || double.IsInfinity(milliseconds)
                || milliseconds < 0 || milliseconds > int.MaxValue) return null;
            return (int)Math.Round(milliseconds, MidpointRounding.AwayFromZero);
        }

        private static bool TryPronunciationNumber(string value, out double number)
        {
            // TTML clock/offset values do not use locale separators, exponents,
            // signs, frame units, or non-finite IEEE values.
            number = 0;
            return value.Length > 0 && value.All(character => char.IsDigit(character) || character == '.')
                && double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number)
                && !double.IsNaN(number) && !double.IsInfinity(number);
        }
    }
}
