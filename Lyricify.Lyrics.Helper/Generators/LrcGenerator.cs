using Lyricify.Lyrics.Helpers.General;
using Lyricify.Lyrics.Models;
using System.Text;

namespace Lyricify.Lyrics.Generators
{
    public static class LrcGenerator
    {
        /// <summary>
        /// 生成 LRC 字符串
        /// </summary>
        /// <param name="lyricsData">用于生成的源歌词数据</param>
        /// <param name="endTimeOutputType">作为行末时间的空行的输出类型</param>
        /// <param name="subLinesOutputType">子行的输出方式</param>
        /// <returns>生成出的 LRC 字符串</returns>
        public static string Generate(LyricsData lyricsData, EndTimeOutputType endTimeOutputType = EndTimeOutputType.Huge, SubLinesOutputType subLinesOutputType = SubLinesOutputType.InMainLine)
        {
            if (lyricsData?.Lines is not { Count: > 0 }) return string.Empty;

            var sb = new StringBuilder();
            var lines = lyricsData.Lines;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if(subLinesOutputType == SubLinesOutputType.InDiffLine)
                {
                    AppendLine(sb, line);
                    if (line.EndTime > 0)
                        if (endTimeOutputType == EndTimeOutputType.All
                            || endTimeOutputType == EndTimeOutputType.Huge && (i + 1 >= lines.Count || lines[i + 1].StartTime - line.EndTime > 5000))
                            sb.AppendLine($"[{StringHelper.FormatTimeMsToTimestampString(line.EndTime.Value)}]");
                    if (line.SubLine is not null)
                    {
                        AppendLine(sb, line.SubLine);
                        if (line.SubLine.EndTime > 0)
                            if (endTimeOutputType == EndTimeOutputType.All
                                || endTimeOutputType == EndTimeOutputType.Huge && (i + 1 >= lines.Count || lines[i + 1].StartTimeWithSubLine - line.SubLine.EndTime > 5000))
                                sb.AppendLine($"[{StringHelper.FormatTimeMsToTimestampString(line.SubLine.EndTime.Value)}]");
                    }
                }
                else
                {
                    if (line.StartTimeWithSubLine.HasValue)
                    {
                        sb.AppendLine($"[{StringHelper.FormatTimeMsToTimestampString(line.StartTimeWithSubLine.Value)}]{line.FullText}");
                        if (line.EndTimeWithSubLine > 0)
                            if (endTimeOutputType == EndTimeOutputType.All
                                || endTimeOutputType == EndTimeOutputType.Huge && (i + 1 >= lines.Count || lines[i + 1].StartTime - line.EndTimeWithSubLine > 5000))
                                sb.AppendLine($"[{StringHelper.FormatTimeMsToTimestampString(line.EndTimeWithSubLine.Value)}]");
                    }
                }

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

        /// <summary>
        /// 子行的输出方式
        /// </summary>
        public enum SubLinesOutputType
        {
            /// <summary>
            /// 通过括号嵌在主行中
            /// </summary>
            InMainLine,

            /// <summary>
            /// 子行单独成行
            /// </summary>
            InDiffLine,
        }
    }
}
