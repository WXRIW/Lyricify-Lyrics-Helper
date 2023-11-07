using Lyricify.Lyrics.Helpers.General;
using Lyricify.Lyrics.Models;
using System.Text;
using static Lyricify.Lyrics.Parsers.LyricifySyllableParser;

namespace Lyricify.Lyrics.Generators
{
    public static class LyricifySyllableGenerator
    {
        // TODO: 仍未完成，不要调用
        public static string Generate(LyricsData lyricsData)
        {
            if (lyricsData?.Lines is not { Count: > 0 }) return string.Empty;

            var sb = new StringBuilder();
            var lines = lyricsData.Lines;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                AppendLine(sb, line);
                if (line.SubLine is not null)
                    AppendLine(sb, line.SubLine);
            }

            return sb.ToString();

            static void AppendLine(StringBuilder sb, ILineInfo line)
            {
                if (line.StartTime.HasValue)
                {
                    //sb.AppendLine($"[{StringHelper.FormatTimeMsToTimestampString(line.StartTime.Value)}]{line.Text}");
                }
            }
        }
    }
}
