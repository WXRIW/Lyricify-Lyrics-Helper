using System.Text.RegularExpressions;

namespace Lyricify.Lyrics.Helpers.Types
{
    public static partial class LyricifyLines
    {
#if NET7_0_OR_GREATER
        [GeneratedRegex(@"^\[\d+,\d+\].*")]
        private static partial Regex LyricsLineCheckRegexGenerated();

        [GeneratedRegex(@"\w+\(\d+,\d+\)")]
        private static partial Regex SyllableRegexGenerated();
#else
        private static Regex LyricsLineCheckRegexGenerated() => new Regex(@"^\[\d+,\d+\].*");
        private static Regex SyllableRegexGenerated() => new Regex(@"\w+\(\d+,\d+\)");
#endif

        private static readonly Regex LyricsLineCheckRegex = LyricsLineCheckRegexGenerated();
        private static readonly Regex SyllableRegex = SyllableRegexGenerated();


        public static bool IsLyricifyLines(string input)
        {
            if (input == null) return false;

            if (input.Contains("[type:LyricifyLines]")) return true;

            var matches = LyricsLineCheckRegex.Matches(input);
            if (matches.Count > 0)
            {
                var syllableMatches = SyllableRegex.Matches(input);
                return syllableMatches.Count <= matches.Count;
            }

            return false;
        }
    }
}
