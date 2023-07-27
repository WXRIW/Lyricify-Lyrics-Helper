using Lyricify.Lyrics.Decrypter.Krc;
using Lyricify.Lyrics.Helpers.General;
using Lyricify.Lyrics.Models;
using Newtonsoft.Json;
using System.Text;

namespace Lyricify.Lyrics.Parsers
{
    public static class KrcParser
    {
        public static LyricsData Parse(string krc)
        {
            var data = new LyricsData
            {
                File = new()
                {
                    Type = LyricsTypes.Krc,
                    SyncTypes = SyncTypes.SyllableSynced,
                    AdditionalInfo = new KrcAdditionalInfo(),
                },
                TrackMetadata = new TrackMetadata()
            };
            var additionalInfo = (KrcAdditionalInfo)data.File.AdditionalInfo;
            additionalInfo.Attributes = new();

            var lyricsLines = GetSplitedKrc(krc).ToList();

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
                    if (attribute.Key == "hash")
                        additionalInfo.Hash = attribute.Value;
                    else
                        additionalInfo.Attributes!.Add(attribute);

                    lyricsLines.RemoveAt(i--);
                }
            }

            // 处理歌词
            var lyrics = ParseLyrics(lyricsLines);
            if (CheckKrcTranslation(krc))
            {
                var lyricsTrans = GetTranslationFromKrc(krc);
                if (lyricsTrans != null)
                {
                    for (int i = 0; i < lyrics.Count && i < lyricsTrans.Count; i++)
                    {
                        string t = lyricsTrans[i];
                        t = t != "//" ? t : "";
                        lyrics[i] = new FullSyllableLineInfo((SyllableLineInfo)lyrics[i], chineseTranslation: t);
                    }
                }
            }

            data.Lines = lyrics;

            return data;
        }

        public static List<ILineInfo> ParseLyrics(string krc)
        {
            var lyricsLines = GetSplitedKrcWithoutInfoLine(krc).ToList();
            var lyrics = ParseLyrics(lyricsLines);

            if (CheckKrcTranslation(krc))
            {
                var lyricsTrans = GetTranslationFromKrc(krc);
                if (lyricsTrans != null)
                {
                    for (int i = 0; i < lyrics.Count && i < lyricsTrans.Count; i++)
                    {
                        string t = lyricsTrans[i];
                        t = t != "//" ? t : "";
                        lyrics[i] = new FullSyllableLineInfo((SyllableLineInfo)lyrics[i], chineseTranslation: t);
                    }
                }
            }

            return lyrics;
        }

        public static List<ILineInfo> ParseLyrics(List<string> lyricsLines)
        {
            var lyrics = new List<ILineInfo>();

            foreach (var line in lyricsLines)
            {
                if (line.StartsWith('['))
                {
                    var l = GetVerbatimLyricsFromKrcLine(line);
                    if (l != null)
                    {
                        lyrics.Add(l);
                    }
                }
            }
            return lyrics;
        }

        /// <summary>
        /// 按行拆分 Krc 歌词
        /// </summary>
        /// <param name="krc"></param>
        /// <returns></returns>
        public static string[] GetSplitedKrc(string krc)
        {
            string[] lines = krc
                .Replace("\r\n", "\n")
                .Replace("\r", "")
                .Split('\n');
            return lines;
        }

        /// <summary>
        /// 按行拆分 Krc 歌词 (移除附加信息)
        /// </summary>
        /// <param name="krc"></param>
        /// <returns></returns>
        public static string[] GetSplitedKrcWithoutInfoLine(string krc)
        {
            string[] lines = krc
                .Replace("\r\n", "\n")
                .Replace("\r", "")
                .Split('\n');
            var stringBuilder = new StringBuilder();
            foreach (string line in lines)
            {
                if (line.StartsWith("[") && line.Length >= 5 && line[1].ToString().IsNumber())
                {
                    stringBuilder.AppendLine(line);
                }
            }
            return stringBuilder.ToString()
                .Replace("\r\n", "\n")
                .Replace("\r", "")
                .Split('\n');
        }

        /// <summary>
        /// 获取 Krc 歌词的 Hash 标签内容
        /// </summary>
        /// <param name="krc"></param>
        /// <returns></returns>
        public static string? GetHashTagFromKrc(string krc)
        {
            string[] lines = krc
                .Replace("\r\n", "\n")
                .Replace("\r", "")
                .Split('\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("[hash:") && line.Length >= 7)
                {
                    var _line = line.Trim();
                    return _line[6..^1];
                }
            }
            return null;
        }

        /// <summary>
        /// 将 Krc 歌词行转换为 SyllableLineInfo
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static SyllableLineInfo? GetVerbatimLyricsFromKrcLine(string line)
        {
            string[] words = line[(line.IndexOf(']') + 1)..].Split(",0>");
            if (words.Length < 1) return null;
            string[] lineTime = line[1..line.IndexOf(']')].Split(',');
            int lineStart = int.Parse(lineTime[0]);

            var syllables = new List<ISyllableInfo>();

            var time = words[0][1..].Split(',');
            int start = int.Parse(time[0]);
            int duration = int.Parse(time[1]);
            for (int i = 1; i < words.Length; i++)
            {
                var word = words[i];
                if (word.Contains('<'))
                {
                    word = word[..word.LastIndexOf("<")];
                }
                syllables.Add(new SyllableInfo()
                {
                    StartTime = lineStart + start,
                    EndTime = lineStart + start + duration,
                    Text = word,
                });
                if (words[i].Contains('<'))
                {
                    time = words[i][(words[i].LastIndexOf("<") + 1)..].Split(',');
                    start = int.Parse(time[0]);
                    duration = int.Parse(time[1]);
                }
            }
            return new(syllables);
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

        #region Translation

        /// <summary>
        /// 检查 KRC 中是否有翻译
        /// </summary>
        public static bool CheckKrcTranslation(string krc)
        {
            if (!krc.Contains("[language:")) return false;

            try
            {
                var language = krc[(krc.IndexOf("[language:") + "[language:".Length)..];
                language = language[..language.IndexOf(']')];
                var decode = Encoding.ASCII.GetString(Convert.FromBase64String(language));

                var translation = JsonConvert.DeserializeObject<KugouTranslation>(decode);
                if (translation!.Content!.Count > 0) return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 提取 KRC 中的翻译
        /// </summary>
        /// <param name="krc">KRC 歌词</param>
        /// <returns>翻译 List，若无翻译，则返回 null</returns>
        public static List<string>? GetTranslationFromKrc(string krc)
        {
            if (!krc.Contains("[language:")) return null;

            var language = krc[(krc.IndexOf("[language:") + "[language:".Length)..];
            language = language[..language.IndexOf(']')];
            var decode = Encoding.ASCII.GetString(Convert.FromBase64String(language));

            var translation = JsonConvert.DeserializeObject<KugouTranslation>(decode);

            if (translation == null || translation!.Content == null || translation!.Content!.Count == 0) return null;

            try
            {
                var result = new List<string>();
                for (int i = 0; i < translation!.Content![0].LyricContent!.Count; i++)
                {
                    result.Add(translation!.Content![0].LyricContent![i]![0]);
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 提取 KRC 中的翻译原始数据
        /// </summary>
        /// <param name="krc">KRC 歌词</param>
        /// <returns>翻译 List，若无翻译，则返回 null</returns>
        public static KugouTranslation? GetTranslationRawFromKrc(string krc)
        {
            if (!krc.Contains("[language:")) return null;

            var language = krc[(krc.IndexOf("[language:") + "[language:".Length)..];
            language = language[..language.IndexOf(']')];
            var decode = Encoding.ASCII.GetString(Convert.FromBase64String(language));

            return JsonConvert.DeserializeObject<KugouTranslation>(decode);
        }

        #endregion Translation
    }
}
