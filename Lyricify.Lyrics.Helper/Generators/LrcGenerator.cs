using Lyricify.Lyrics.Helpers.General;
using Lyricify.Lyrics.Models;
using System.Text;

namespace Lyricify.Lyrics.Generators
{
    public static class LrcGenerator
    {
        // TODO: 本方法未经测试
        public static string Generate(LyricsData lyricsData, EndTimeOutputType endTimeOutputType = EndTimeOutputType.Huge)
        {
            if (lyricsData?.Lines is not { Count: > 0 }) return string.Empty;

            var sb = new StringBuilder();
            var lines = lyricsData.Lines;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                AppendLine(sb, line);
                if (line.EndTime.HasValue && line.EndTime > 0)
                    if (endTimeOutputType == EndTimeOutputType.All
                        || endTimeOutputType == EndTimeOutputType.Huge && (i + 1 >= lines.Count || lines[i + 1].StartTime - line.EndTime > 5000))
                        sb.AppendLine($"[{StringHelper.FormatTimeMsToTimestampString(line.EndTime.Value)}]{line.Text}");
                if (line.SubLine is not null)
                    AppendLine(sb, line.SubLine);
            }

            return sb.ToString();

            static void AppendLine(StringBuilder sb, ILineInfo line)
            {
                if (line.StartTime.HasValue)
                {
                    sb.AppendLine($"[{StringHelper.FormatTimeMsToTimestampString(line.StartTime.Value)}]{line.Text}");
                }
            }
        }

        /// <summary>
        /// 作为行末时间的空行的输出类型
        /// </summary>
        public enum EndTimeOutputType
        {
            /// <summary>
            /// 不输出作为行末时间的空行
            /// </summary>
            None,

            /// <summary>
            /// 输出间距 (5s 以上) 较大的行末时间空行
            /// </summary>
            Huge,

            /// <summary>
            /// 输出所有行末时间空行
            /// </summary>
            All,
        }
    }
}
