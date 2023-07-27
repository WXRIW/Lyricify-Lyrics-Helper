using Lyricify.Lyrics.Helpers.General;
using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Parsers
{
    public static class LyricifyLinesParser
    {
        public static LyricsData Parse(string lyrics)
        {
            var lyricsLines = lyrics.Replace("[type:LyricifyLines]", "").Trim().Split('\n').ToList();
            var data = new LyricsData
            {
                TrackMetadata = new TrackMetadata(),
                File = new()
                {
                    SyncTypes = SyncTypes.LineSynced,
                    Type = LyricsTypes.LyricifyLines,
                    AdditionalInfo = new GeneralAdditionalInfo()
                    {
                        Attributes = new(),
                    }
                }
            };

            // 处理 Attributes
            for (int i = 0; i < lyricsLines.Count; i++)
            {
                if (IsAttributeLine(lyricsLines[i]))
                {
                    var attribute = GetAttribute(lyricsLines[i]);
                    switch (attribute.Key)
                    {
                        case "ar":
                            data.TrackMetadata.Artist = attribute.Value;
                            break;
                        case "al":
                            data.TrackMetadata.Album = attribute.Value;
                            break;
                        case "ti":
                            data.TrackMetadata.Title = attribute.Value;
                            break;
                        case "length":
                            data.TrackMetadata.DurationMs = attribute.Value;
                            break;
                    }
                    ((GeneralAdditionalInfo)data.File.AdditionalInfo).Attributes!.Add(attribute);

                    lyricsLines.RemoveAt(i--);
                }
            }

            var lines = ParseLyrics(lyricsLines);
            data.Lines = lines.Cast<ILineInfo>().ToList();

            return data;
        }

        public static List<LineInfo> ParseLyrics(List<string> lines)
        {
            var lyricsArray = new List<LineInfo>();
            foreach (var line in lines)
            {
                if (!line.StartsWith('[') || !line.Contains(',') || !line.Contains(']')) continue;
                try
                {
                    int begin = int.Parse(line.Between("[", ","));
                    int end = int.Parse(line.Between(",", "]"));
                    string text = line[(line.IndexOf(']') + 1)..].Trim();
                    lyricsArray.Add(new()
                    {
                        Text = text,
                        StartTime = begin,
                        EndTime = end,
                    });
                }
                catch { }
            }
            return lyricsArray;
        }

        /// <summary>
        /// 是否是 Attribute 信息行
        /// </summary>
        public static bool IsAttributeLine(string line) => line.StartsWith('[') && line.EndsWith(']') && line.Contains(':');

        /// <summary>
        /// 获取 Attribute 信息
        /// </summary>
        public static KeyValuePair<string, string> GetAttribute(string line)
        {
            string key = line.Between("[", ":");
            string value = line[(line.IndexOf(':') + 1)..^1];
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
