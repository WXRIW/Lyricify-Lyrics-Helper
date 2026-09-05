using Lyricify.Lyrics.Models;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Lyricify.Lyrics.Helpers.Types
{
    internal static class LyricsTypeDetector
    {
        private const string TtmlNamespace = "http://www.w3.org/ns/ttml";

        private const RegexOptions LineOptions =
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline;

        // 格式：一个或多个 [分:秒.毫秒] 时间戳，毫秒部分可省略，也支持使用冒号分隔毫秒。
        // 示例：[00:12.345]Hello world
        private static readonly Regex LrcLine = new(
            @"^[ \t]*(?:\[\d+:\d{1,2}(?:[.:]\d{1,3})?\])+[^\r\n]*",
            LineOptions);

        // 格式：[开始时间,结束时间]歌词文本；这是 Lyricify Lines 与逐字格式共有的行头。
        // 示例：[12000,15000]Hello world
        private static readonly Regex BracketedLine = new(
            @"^[ \t]*\[\d+,\d+\][^\r\n]*",
            LineOptions);

        // 格式：[行开始时间,行时长]文本(字开始时间,字时长)。
        // 示例：[12000,3000]Hel(12000,400)lo(12400,500)
        private static readonly Regex QrcLine = new(
            @"^[ \t]*\[\d+,\d+\][^\r\n]*\(-?\d+,\d+\)[^\r\n]*",
            LineOptions);

        // 格式：[行开始时间,行时长]<相对开始时间,字时长,保留值>文本。
        // 示例：[12000,3000]<0,400,0>Hel<400,500,0>lo
        private static readonly Regex KrcLine = new(
            @"^[ \t]*\[\d+,\d+\]<-?\d+,\d+,\d+>[^\r\n]*",
            LineOptions);

        // 格式：[行开始时间,行时长](字开始时间,字时长,保留值)文本。
        // 示例：[12000,3000](12000,400,0)Hel(12400,500,0)lo
        private static readonly Regex YrcLine = new(
            @"^[ \t]*\[\d+,\d+\]\(-?\d+,\d+,\d+\)[^\r\n]*",
            LineOptions);

        // 格式：[行属性]文本(字开始时间,字时长)，行属性表示主/背景人声及对齐方式。
        // 示例：[4]Hel(12000,400)lo(12400,500)
        private static readonly Regex LyricifySyllableLine = new(
            @"^[ \t]*\[\d+\][^\r\n]*\(-?\d+,\d+\)[^\r\n]*",
            LineOptions);

        // 匹配任意受支持的逐字时间片段，用于避免把逐字歌词误判为 Lyricify Lines。
        // 示例：(12000,400)、(12000,400,0)、<0,400,0>
        private static readonly Regex AnySyllableTiming = new(
            @"(?:\(-?\d+,\d+(?:,\d+)?\)|<-?\d+,\d+,\d+>)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // 匹配无法作为标准 XML 解析时的 QRC Full 特征节点与歌词属性。
        // 示例：<Lyric_1 LyricContent="[0,100]Hi(0,100)" />
        private static readonly Regex QrcFullFallback = new(
            @"<Lyric_1\b[^>]*\bLyricContent\s*=",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        internal static LyricsRawTypes Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return LyricsRawTypes.Unknown;

            var structuredType = GetJsonType(input);
            if (structuredType != LyricsRawTypes.Unknown) return structuredType;

            structuredType = GetXmlType(input);
            if (structuredType != LyricsRawTypes.Unknown) return structuredType;

            if (HasLyricifyLinesTypeMarker(input)) return LyricsRawTypes.LyricifyLines;
            if (IsKrc(input)) return LyricsRawTypes.Krc;
            if (IsYrc(input)) return LyricsRawTypes.Yrc;
            if (IsLyricifySyllable(input)) return LyricsRawTypes.LyricifySyllable;
            if (IsQrc(input)) return LyricsRawTypes.Qrc;
            if (IsLyricifyLines(input)) return LyricsRawTypes.LyricifyLines;
            if (IsLrc(input)) return LyricsRawTypes.Lrc;

            return LyricsRawTypes.Unknown;
        }

        internal static bool IsLrc(string input) =>
            input != null && LrcLine.IsMatch(input);

        internal static bool IsLyricifyLines(string input)
        {
            if (input == null) return false;
            if (HasLyricifyLinesTypeMarker(input)) return true;

            return BracketedLine.IsMatch(input) && !AnySyllableTiming.IsMatch(input);
        }

        internal static bool IsLyricifySyllable(string input) =>
            input != null && LyricifySyllableLine.IsMatch(input);

        internal static bool IsQrc(string input) =>
            input != null && QrcLine.IsMatch(input);

        internal static bool IsQrcFull(string input) =>
            input != null && GetXmlType(input) == LyricsRawTypes.QrcFull;

        internal static bool IsKrc(string input) =>
            input != null && KrcLine.IsMatch(input);

        internal static bool IsYrc(string input) =>
            input != null && YrcLine.IsMatch(input);

        internal static bool IsYrcFull(string input) =>
            input != null && GetJsonType(input) == LyricsRawTypes.YrcFull;

        internal static bool IsTtml(string input) =>
            input != null && GetXmlType(input) == LyricsRawTypes.Ttml;

        internal static bool IsAppleJson(string input) =>
            input != null && GetJsonType(input) == LyricsRawTypes.AppleJson;

        internal static bool IsSpotify(string input) =>
            input != null && GetJsonType(input) == LyricsRawTypes.Spotify;

        internal static bool IsMusixmatch(string input) =>
            input != null && GetJsonType(input) == LyricsRawTypes.Musixmatch;

        private static bool HasLyricifyLinesTypeMarker(string input) =>
            input.IndexOf("[type:LyricifyLines]", StringComparison.OrdinalIgnoreCase) >= 0;

        private static LyricsRawTypes GetJsonType(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.TrimStart().FirstOrDefault() != '{')
            {
                return LyricsRawTypes.Unknown;
            }

            JObject root;
            try
            {
                root = JObject.Parse(input);
            }
            catch
            {
                return LyricsRawTypes.Unknown;
            }

            if (IsAppleJson(root)) return LyricsRawTypes.AppleJson;
            if (IsSpotify(root)) return LyricsRawTypes.Spotify;
            if (IsMusixmatch(root)) return LyricsRawTypes.Musixmatch;
            if (IsYrcFull(root)) return LyricsRawTypes.YrcFull;

            return LyricsRawTypes.Unknown;
        }

        private static LyricsRawTypes GetXmlType(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.TrimStart().FirstOrDefault() != '<')
            {
                return LyricsRawTypes.Unknown;
            }

            try
            {
                var document = XDocument.Parse(input, LoadOptions.PreserveWhitespace);
                var root = document.Root;
                if (root == null) return LyricsRawTypes.Unknown;

                if (root.DescendantsAndSelf().Any(element =>
                        element.Name.LocalName.Equals("Lyric_1", StringComparison.OrdinalIgnoreCase)
                        && element.Attributes().Any(attribute =>
                            attribute.Name.LocalName.Equals("LyricContent", StringComparison.OrdinalIgnoreCase))))
                {
                    return LyricsRawTypes.QrcFull;
                }

                if (root.Name.LocalName.Equals("tt", StringComparison.OrdinalIgnoreCase)
                    && root.Name.NamespaceName.Equals(TtmlNamespace, StringComparison.Ordinal))
                {
                    return LyricsRawTypes.Ttml;
                }
            }
            catch
            {
                if (QrcFullFallback.IsMatch(input)) return LyricsRawTypes.QrcFull;
            }

            return LyricsRawTypes.Unknown;
        }

        private static bool IsAppleJson(JObject root)
        {
            if (Get(root, "data") is not JArray data) return false;

            foreach (var item in data.OfType<JObject>())
            {
                if (IsSyllableLyricsItem(item)) return true;

                if (Get(item, "relationships") is not JObject relationships
                    || Get(relationships, "syllable-lyrics") is not JObject syllableLyrics
                    || Get(syllableLyrics, "data") is not JArray relationshipData)
                {
                    continue;
                }

                if (relationshipData.OfType<JObject>().Any(IsSyllableLyricsItem)) return true;
            }

            return false;
        }

        private static bool IsSyllableLyricsItem(JObject item)
        {
            var type = Get(item, "type")?.Value<string>();
            if (!string.IsNullOrEmpty(type)
                && !type.Equals("syllable-lyrics", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (Get(item, "attributes") is not JObject attributes) return false;
            return HasNonEmptyString(attributes, "ttml")
                || HasNonEmptyString(attributes, "ttmlLocalizations");
        }

        private static bool IsSpotify(JObject root) =>
            Get(root, "lyrics") is JObject lyrics
            && Get(lyrics, "syncType")?.Type == JTokenType.String
            && Get(lyrics, "lines") is JArray;

        private static bool IsMusixmatch(JObject root)
        {
            if (Get(root, "message") is not JObject message
                || Get(message, "body") is not JObject body
                || Get(body, "macro_calls") is not JObject calls)
            {
                return false;
            }

            return Get(calls, "track.richsync.get") != null
                || Get(calls, "track.subtitles.get") != null
                || Get(calls, "track.lyrics.get") != null;
        }

        private static bool IsYrcFull(JObject root) =>
            Get(root, "yrc") is JObject yrc
            && Get(yrc, "lyric")?.Type == JTokenType.String;

        private static bool HasNonEmptyString(JObject value, string propertyName) =>
            !string.IsNullOrWhiteSpace(Get(value, propertyName)?.Value<string>());

        private static JToken? Get(JObject value, string propertyName) =>
            value.GetValue(propertyName, StringComparison.OrdinalIgnoreCase);
    }
}
