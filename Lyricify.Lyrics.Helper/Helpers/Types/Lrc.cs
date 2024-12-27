using System.Text.RegularExpressions;

namespace Lyricify.Lyrics.Helpers.Types
{
    public static partial class Lrc
    {
#if NET7_0_OR_GREATER
        [GeneratedRegex(@"\[\d+:\d+\.\d+\](.+)")]
        private static partial Regex IsLrcRegexGenerated();
#else
        private static Regex IsLrcRegexGenerated() => new Regex(@"\[\d+:\d+\.\d+\](.+)");
#endif

        private static readonly Regex IsLrcRegex = IsLrcRegexGenerated();

        public static bool IsLrc(string input)
        {
            if (input == null) return false;

            MatchCollection matches = IsLrcRegex.Matches(input);
            return matches.Count > 0;
        }
    }
}
