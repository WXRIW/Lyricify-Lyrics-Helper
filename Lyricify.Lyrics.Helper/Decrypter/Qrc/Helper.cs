using Newtonsoft.Json;
using System.Xml;

namespace Lyricify.Lyrics.Decrypter.Qrc
{
    public class Helper
    {
        /// <summary>
        /// 通过 Mid 获取解密后的歌词
        /// </summary>
        /// <param name="id">QQ 音乐歌曲 Mid</param>
        /// <returns></returns>
        public static QqLyricsResponse? GetLyricsByMid(string mid)
        {
            var song = GetSong(mid).Result;
            if (song == null || song.Data is { Length: > 0 }) return null;
            var id = song.Data?[0].Id;
            return GetLyricsAsync(id!).Result;
        }

        /// <summary>
        /// 通过 Mid 获取解密后的歌词
        /// </summary>
        /// <param name="id">QQ 音乐歌曲 Mid</param>
        /// <returns></returns>
        public static async Task<QqLyricsResponse?> GetLyricsByMidAsync(string mid)
        {
            var song = await GetSong(mid);
            if (song == null || song.Data is { Length: > 0 }) return null;
            var id = song.Data?[0].Id;
            return await GetLyricsAsync(id!);
        }

        /// <summary>
        /// 通过 ID 获取解密后的歌词
        /// </summary>
        /// <param name="id">QQ 音乐歌曲 ID</param>
        /// <returns></returns>
        public static QqLyricsResponse? GetLyrics(string id)
        {
            return GetLyricsAsync(id).Result;
        }

        /// <summary>
        /// 通过 ID 获取解密后的歌词
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<QqLyricsResponse?> GetLyricsAsync(string id)
        {
            var resp = await PostAsync("https://c.y.qq.com/qqmusic/fcgi-bin/lyric_download.fcg", new Dictionary<string, string>
                {
                    { "version", "15" },
                    { "miniversion", "82" },
                    { "lrctype", "4" },
                    { "musicid", id },
                });

            resp = resp.Replace("<!--", "").Replace("-->", "");

            var dict = new Dictionary<string, XmlNode>();

            XmlUtils.RecursionFindElement(XmlUtils.Create(resp), VerbatimXmlMappingDict, dict);

            var result = new QqLyricsResponse
            {
                Lyrics = "",
                Trans = ""
            };

            foreach (var pair in dict)
            {
                var text = pair.Value.InnerText;

                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                string decompressText;
                try
                {
                    decompressText = Decrypter.DecryptLyrics(text) ?? "";
                }
                catch
                {
                    continue;
                }

                var s = "";
                if (decompressText.Contains("<?xml"))
                {
                    var doc = XmlUtils.Create(decompressText);

                    var subDict = new Dictionary<string, XmlNode>();

                    XmlUtils.RecursionFindElement(doc, VerbatimXmlMappingDict, subDict);

                    if (subDict.TryGetValue("lyric", out var d))
                    {
                        s = d.Attributes?["LyricContent"]?.InnerText;
                    }
                }
                else
                {
                    s = decompressText;
                }

                if (!string.IsNullOrWhiteSpace(s))
                {
                    switch (pair.Key)
                    {
                        case "orig":
                            result.Lyrics = s;// LyricUtils.DealVerbatimLyric(s, SearchSourceEnum.QQ_MUSIC);
                            break;
                        case "ts":
                            result.Trans = s;// LyricUtils.DealVerbatimLyric(s, SearchSourceEnum.QQ_MUSIC);
                            break;
                    }
                }
            }

            if (result.Lyrics == "" && result.Trans == "")
            {
                return null;
            }
            return result;
        }

        protected static async Task<SongResponse?> GetSong(string mid)
        {
            const string callBack = "getOneSongInfoCallback";

            var data = new Dictionary<string, string>
            {
                { "songmid", mid },
                { "tpl", "yqq_song_detail" },
                { "format", "jsonp" },
                { "callback", callBack },
                { "g_tk", "5381" },
                { "jsonpCallback", callBack },
                { "loginUin", "0" },
                { "hostUin", "0" },
                { "outCharset", "utf8" },
                { "notice", "0" },
                { "platform", "yqq" },
                { "needNewCode", "0" },
            };

            var json = await PostAsync("https://c.y.qq.com/v8/fcg-bin/fcg_play_single_song.fcg", data);

            try
            {
                return JsonConvert.DeserializeObject<SongResponse>(json);
            }
            catch
            {
                return null;
            }
        }

        public readonly static HttpClient Client = new();

        protected static string Post(string url, Dictionary<string, string> paramDict)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            request.Headers.Add("Referer", "https://c.y.qq.com/");
            request.Headers.Add("UserAgent", UserAgent);
            request.Headers.Add("Cookie", Cookie);
            request.Content = new FormUrlEncodedContent(paramDict);
            var response = Client.SendAsync(request).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        protected static async Task<string> PostAsync(string url, Dictionary<string, string> paramDict)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            request.Headers.Add("Referer", "https://c.y.qq.com/");
            request.Headers.Add("UserAgent", UserAgent);
            request.Headers.Add("Cookie", Cookie);
            request.Content = new FormUrlEncodedContent(paramDict);
            var response = await Client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";

        public const string Cookie = "os=pc;osver=Microsoft-Windows-10-Professional-build-16299.125-64bit;appver=2.0.3.131777;channel=netease;__remember_me=true";

        private static readonly Dictionary<string, string> VerbatimXmlMappingDict = new()
        {
            { "content", "orig" }, // 原文
            { "contentts", "ts" }, // 译文
            { "contentroma", "roma" }, // 罗马音
            { "Lyric_1", "lyric" }, // 解压后的内容
        };
    }
}
