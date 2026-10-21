using System.Collections.ObjectModel;
using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Helpers
{
    /// <summary>Reads multilingual pronunciation from the library's format-independent lyric models.</summary>
    /// <remarks>Source-format parsing is handled by the individual parsers.</remarks>
    public static class PronunciationHelper
    {
        private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Empty =
            new ReadOnlyDictionary<string, IReadOnlyList<string>>(new Dictionary<string, IReadOnlyList<string>>());

        public static IReadOnlyDictionary<string, IReadOnlyList<string>> GetPronunciations(this ILineInfo line) => line switch
        {
            FullLineInfo full => ReadOnly(full.Pronunciations),
            FullSyllableLineInfo full => full.Pronunciations,
            SyllableLineInfo syllable => Aggregate(syllable.Syllables),
            _ => Empty
        };

        public static IReadOnlyDictionary<string, IReadOnlyList<string>> GetPronunciations(this ISyllableInfo syllable) => syllable switch
        {
            SyllableInfo leaf => ReadOnly(leaf.Pronunciations),
            FullSyllableInfo group => group.Pronunciations,
            _ => Empty
        };

        public static string? GetPronunciation(this ILineInfo line, string? language = null)
        {
            if (language == null && line is IFullLineInfo full) return full.Pronunciation;
            return GetText(line.GetPronunciations(), language);
        }

        public static string? GetPronunciation(this ISyllableInfo syllable, string? language = null) =>
            GetText(syllable.GetPronunciations(), language);

        private static string? GetText(IReadOnlyDictionary<string, IReadOnlyList<string>> data, string? language)
        {
            if (language == null) return GetDefaultPronunciation(data, null);
            foreach (var pair in data)
                if (string.Equals(pair.Key, language, StringComparison.OrdinalIgnoreCase))
                    return string.Concat(pair.Value);
            return null;
        }

        internal static string? GetDefaultPronunciation(Dictionary<string, List<string>>? data, string? language) =>
            GetDefaultPronunciation(ReadOnly(data), language);

        internal static string? GetDefaultPronunciation(IReadOnlyDictionary<string, IReadOnlyList<string>> data, string? language)
        {
            if (language != null)
            {
                var selected = GetText(data, language);
                if (selected != null) return selected;
            }
            if (data.Count == 0) return null;
            var preferred = data.FirstOrDefault(pair => pair.Key.Split('-').Any(part =>
                string.Equals(part, "Latn", StringComparison.OrdinalIgnoreCase)));
            return string.Concat(preferred.Value ?? data.First().Value);
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<string>> ReadOnly(Dictionary<string, List<string>>? data)
        {
            if (data == null || data.Count == 0) return Empty;
            var result = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in data) result[pair.Key] = new List<string>(pair.Value).AsReadOnly();
            return new ReadOnlyDictionary<string, IReadOnlyList<string>>(result);
        }

        internal static IReadOnlyDictionary<string, IReadOnlyList<string>> Aggregate(
            IEnumerable<ISyllableInfo>? syllables, Dictionary<string, List<string>>? overrides = null)
        {
            var data = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var syllable in syllables ?? Enumerable.Empty<ISyllableInfo>())
            {
                foreach (var pair in syllable.GetPronunciations())
                {
                    if (!data.TryGetValue(pair.Key, out var values)) data[pair.Key] = values = new List<string>();
                    values.AddRange(pair.Value);
                }
            }
            if (overrides != null)
                foreach (var pair in overrides) data[pair.Key] = new List<string>(pair.Value);
            return ReadOnly(data);
        }

        internal static SyllableInfo Clone(SyllableInfo source, string? text = null)
        {
            var copy = new SyllableInfo(text ?? source.Text, source.StartTime, source.EndTime);
            CopyPronunciations(source, copy);
            return copy;
        }

        internal static void CopyPronunciations(ISyllableInfo source, ISyllableInfo target)
        {
            var data = source is FullSyllableInfo sourceGroup && target is FullSyllableInfo
                ? ReadOnly(sourceGroup.GroupPronunciations)
                : source.GetPronunciations();
            var destination = target switch
            {
                SyllableInfo leaf => leaf.Pronunciations,
                FullSyllableInfo group => group.GroupPronunciations,
                _ => null
            };
            Copy(data, destination);
        }

        internal static void CopyPronunciations(ILineInfo source, ILineInfo target)
        {
            var data = target is FullSyllableLineInfo && source is SyllableLineInfo
                ? source is FullSyllableLineInfo sourceFull ? ReadOnly(sourceFull.UnassignedPronunciations) : Empty
                : source.GetPronunciations();
            var destination = target switch
            {
                FullLineInfo full => full.Pronunciations,
                FullSyllableLineInfo full => full.UnassignedPronunciations,
                _ => null
            };
            Copy(data, destination);
            var language = source switch
            {
                FullLineInfo full => full.PronunciationLanguage,
                FullSyllableLineInfo full => full.PronunciationLanguage,
                _ => null
            };
            var explicitValue = source switch
            {
                FullLineInfo full => full.PronunciationOverride,
                FullSyllableLineInfo full => full.PronunciationOverride,
                IFullLineInfo full => full.Pronunciation,
                _ => null
            };
            if (target is FullLineInfo targetLine)
            {
                targetLine.PronunciationLanguage = language;
                targetLine.PronunciationOverride = explicitValue;
            }
            else if (target is FullSyllableLineInfo targetSyllable)
            {
                targetSyllable.PronunciationLanguage = language;
                targetSyllable.PronunciationOverride = explicitValue;
            }
        }

        private static void Copy(IReadOnlyDictionary<string, IReadOnlyList<string>> source, Dictionary<string, List<string>>? target)
        {
            if (target == null) return;
            target.Clear();
            foreach (var pair in source) target[pair.Key] = new List<string>(pair.Value);
        }
    }
}
