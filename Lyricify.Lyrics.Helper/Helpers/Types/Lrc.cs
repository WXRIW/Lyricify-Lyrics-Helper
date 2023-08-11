using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lyricify.Lyrics.Helpers.Types
{
    public class Lrc
    {
        public static bool IsLrc(string input)
        {
            var pattern = @"\[\d+:\d+\.\d+\](.+)";
            var regex = new Regex(pattern);

            MatchCollection matches = regex.Matches(input);
            return matches.Count > 0;
        }
    }
}
