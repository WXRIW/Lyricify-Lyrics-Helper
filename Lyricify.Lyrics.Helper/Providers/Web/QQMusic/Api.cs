using Lyricify.Lyrics.Decrypter.Qrc;
using Lyricify.Lyrics.Helpers.General;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Lyricify.Lyrics.Providers.Web.QQMusic
{
    public class Api : BaseApi
    {
        protected override string HttpRefer => "https://c.y.qq.com/";

        protected override Dictionary<string, string>? AdditionalHeaders => null;

        private static readonly DateTime _dtFrom = new(1970, 1, 1, 8, 0, 0, 0, DateTimeKind.Local);

        private static readonly Dictionary<string, string> VerbatimXmlMappingDict = new()
        {
            { "content", "orig" }, // 原文
            { "contentts", "ts" }, // 译文
            { "contentroma", "roma" }, // 罗马音
            { "Lyric_1", "lyric" }, // 解压后的内容
        };

        // 搜索类型
        public enum SearchTypeEnum
        {
            [Description("单曲")] SONG_ID = 0,
            [Description("专辑")] ALBUM_ID = 1,
            [Description("歌单")] PLAYLIST_ID = 2,
        }

        public async Task<MusicFcgApiResult?> Search(string keyword, SearchTypeEnum searchType)
        {
            // 0单曲 2专辑 1歌手 3歌单 7歌词 12mv
            var type = searchType switch
            {
                SearchTypeEnum.SONG_ID => 0,
                SearchTypeEnum.ALBUM_ID => 2,
                SearchTypeEnum.PLAYLIST_ID => 3,
                _ => 0,
            };

            var data = new SearchRequestModel()
            {
                req_1 = new SearchRequestModel.SearchRequestModel_req_1()
                {
                    method = "DoSearchForQQMusicDesktop",
                    module = "music.search.SearchCgiService",
                    param = new SearchRequestModel.SearchRequestModel_req_1_param()
                    {
                        num_per_page = "20",
                        page_num = "1",
                        query = keyword,
                        search_type = type,
                    }
                }
            };

            var resp = await PostJsonAsync("https://u.y.qq.com/cgi-bin/musicu.fcg", data);

            return resp.ToEntity<MusicFcgApiResult>();
        }

        public async Task<MusicFcgApiAlternativeResult?> SearchAlternative(string keyword)
        {
            string data = "{\"music.search.SearchCgiService\": {\"method\": \"DoSearchForQQMusicDesktop\",\"module\": \"music.search.SearchCgiService\",\"param\": {\"num_per_page\": 10,\"page_num\": 1,\"query\": \"" + keyword + "\",\"search_type\": 0}}}";

            var resp = await PostAsync("https://u.y.qq.com/cgi-bin/musicu.fcg", data);

            return resp.ToEntity<MusicFcgApiAlternativeResult>();
        }

        public async Task<AlbumResult?> GetAlbum(string albumMid)
        {
            var data = new Dictionary<string, string>
            {
                { "albummid", albumMid }
            };

            var resp = await PostAsync("https://c.y.qq.com/v8/fcg-bin/fcg_v8_album_info_cp.fcg", data);

            return resp.ToEntity<AlbumResult>();
        }

        public async Task<AlbumSongListResult?> GetAlbumSongList(string mid, int page = 1, int pageSize = 1000)
        {
            var data = new GetAlbumSongListRequestModel()
            {
                comm = new GetAlbumSongListRequestModel.GetAlbumSongListRequestModel_comm()
                {
                    ct = 24,
                    cv = 10000
                },
                albumSonglist = new GetAlbumSongListRequestModel.GetAlbumSongListRequestModel_albumSonglist()
                {
                    method = "GetAlbumSongList",
                    param = new GetAlbumSongListRequestModel.GetAlbumSongListRequestModel_albumSonglist_param()
                    {
                        albumMid = mid,
                        albumID = 0,
                        begin = (page - 1) * pageSize,
                        num = pageSize,
                        order = 2
                    },
                    module = "music.musichallAlbum.AlbumSongList"
                }
            };

            var resp = await PostJsonAsync("https://u.y.qq.com/cgi-bin/musicu.fcg?g_tk=5381&format=json&inCharset=utf8&outCharset=utf-8", data);

            return resp.ToEntity<AlbumSongListResult>();
        }

        public async Task<SingerSongResult?> GetSingerSongs(string singerMid, int page = 1, int pageSize = 20)
        {
            var data = new GetSingerSongsRequestModel()
            {
                comm = new GetSingerSongsRequestModel.GetSingerSongsRequestModel_comm()
                {
                    ct = 24,
                    cv = 0
                },
                singer = new GetSingerSongsRequestModel.GetSingerSongsRequestModel_singer()
                {
                    method = "get_singer_detail_info",
                    param = new GetSingerSongsRequestModel.GetSingerSongsRequestModel_singer_param()
                    {
                        sort = 5,
                        singermid = singerMid,
                        sin = (page - 1) * pageSize,
                        num = pageSize
                    },
                    module = "music.web_singer_info_svr"
                }
            };

            var resp = await PostJsonAsync("http://u.y.qq.com/cgi-bin/musicu.fcg", data);

            return resp.ToEntity<SingerSongResult>();
        }

        public async Task<ToplistResult?> GetToplist(int id = 4, int page = 1, int pageSize = 100, string? period = null)
        {
            string timeType = id switch
            {
                4 or 27 or 62 => "yyyy-MM-dd",
                _ => "yyyy-W",
            };
            string postPeriod = period ?? DateTime.Now.ToString(timeType);

            var data = new GetToplistRequestModel()
            {
                detail = new GetToplistRequestModel.GetToplistRequestModel_detail()
                {
                    module = "musicToplist.ToplistInfoServer",
                    method = "GetDetail",
                    param = new GetToplistRequestModel.GetToplistRequestModel_detail_param()
                    {
                        topId = id,
                        offset = (page - 1) * pageSize,
                        num = pageSize,
                        period = postPeriod,
                    },
                },
                comm = new GetToplistRequestModel.GetToplistRequestModel_comm()
                {
                    ct = 24,
                    cv = 0
                }
            };

            var resp = await PostJsonAsync("https://u.y.qq.com/cgi-bin/musicu.fcg", data);

            return resp.ToEntity<ToplistResult>();
        }

        public async Task<PlaylistResult?> GetPlaylist(string playlistId)
        {
            var data = new Dictionary<string, string>
            {
                { "disstid", playlistId },
                { "format", "json" },
                { "outCharset", "utf8" },
                { "type", "1" },
                { "json", "1" },
                { "utf8", "1" },
                { "onlysong", "0" }, // 返回歌曲明细
                { "new_format", "1" },
            };
            var resp = await PostAsync("https://c.y.qq.com/qzone/fcg-bin/fcg_ucc_getcdinfo_byids_cp.fcg", data);

            return resp.ToEntity<PlaylistResult>();
        }

        /// <summary>
        /// query music song
        /// </summary>
        /// <param name="id">query song by id, support songId and midId, eg: 001RaE0n4RrGX9 or 204422870</param>
        /// <returns>music song</returns>
        public async Task<SongResult?> GetSong(string id)
        {
            const string callBack = "getOneSongInfoCallback";

            var data = new Dictionary<string, string>
            {
                { id.IsNumber() ? "songid" : "songmid", id },
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

            var resp = await PostAsync("https://c.y.qq.com/v8/fcg-bin/fcg_play_single_song.fcg", data);

            return ResolveRespJson(callBack, resp).ToEntity<SongResult>();
        }

        public async Task<LyricResult?> GetLyric(string songMid)
        {
            var currentMillis = (DateTime.Now.ToLocalTime().Ticks - _dtFrom.Ticks) / 10000;

            const string callBack = "MusicJsonCallback_lrc";

            var data = new Dictionary<string, string>
            {
                { "callback", "MusicJsonCallback_lrc" },
                { "pcachetime", currentMillis.ToString() },
                { "songmid", songMid },
                { "g_tk", "5381" },
                { "jsonpCallback", callBack },
                { "loginUin", "0" },
                { "hostUin", "0" },
                { "format", "jsonp" },
                { "inCharset", "utf8" },
                { "outCharset", "utf8" },
                { "notice", "0" },
                { "platform", "yqq" },
                { "needNewCode", "0" },
            };

            var resp = await PostAsync("https://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric_new.fcg", data);

            var result = ResolveRespJson(callBack, resp).ToEntity<LyricResult>();

            return result?.Decode();
        }

        /// <summary>
        /// 通过 ID 获取解密后的歌词
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<QqLyricsResponse?> GetLyricsAsync(string id)
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
                    decompressText = Decrypter.Qrc.Decrypter.DecryptLyrics(text) ?? "";
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
                            result.Lyrics = s;
                            break;
                        case "ts":
                            result.Trans = s;
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

        public async Task<string> GetSongLink(string songMid)
        {
            var guid = GetGuid();

            var data = new GetSongLinkRequestModel()
            {
                comm = new GetSongLinkRequestModel.GetSongLinkRequestModel_comm()
                {
                    uin = 0,
                    format = "json",
                    ct = 24,
                    cv = 0,
                },
                req = new GetSongLinkRequestModel.GetSongLinkRequestModel_req()
                {
                    method = "GetCdnDispatch",
                    module = "CDN.SrfCdnDispatchServer",
                    param = new GetSongLinkRequestModel.GetSongLinkRequestModel_req_param()
                    {
                        guid = guid,
                        calltype = "0",
                        userip = ""
                    }
                },
                req0 = new GetSongLinkRequestModel.GetSongLinkRequestModel_req0()
                {
                    method = "CgiGetVkey",
                    module = "vkey.GetVkeyServer",
                    param = new GetSongLinkRequestModel.GetSongLinkRequestModel_req0_param()
                    {
                        guid = "8348972662",
                        songmid = new[] { songMid },
                        songtype = new[] { 1 },
                        uin = "0",
                        loginflag = 1,
                        platform = "20"
                    }
                }
            };

            var resp = await PostJsonAsync("https://u.y.qq.com/cgi-bin/musicu.fcg", data);

            var res = resp.ToEntity<MusicFcgApiResult>();

            var link = "";
            if (res?.Code == 0 && res?.Req.Code == 0 && res?.Req_0.Code == 0)
            {
                link = res.Req.Data.Sip[0] + res.Req_0.Data.Midurlinfo[0].Purl;
            }

            return link;
        }

        private static string ResolveRespJson(string callBackSign, string val)
        {
            if (!val.StartsWith(callBackSign))
            {
                return string.Empty;
            }

            var jsonStr = val.Replace(callBackSign + "(", string.Empty);
            return jsonStr.Remove(jsonStr.Length - 1);
        }

        protected virtual string GetGuid()
        {
            var guid = new StringBuilder(10);
            var r = new Random();
            for (var i = 0; i < 10; i++)
            {
                guid.Append(Convert.ToString(r.Next(10)));
            }

            return guid.ToString();
        }


        internal class SearchRequestModel
        {
            public SearchRequestModel_req_1 req_1 { get; set; }

            public class SearchRequestModel_req_1
            {
                public string method { get; set; }

                public string module { get; set; }

                public SearchRequestModel_req_1_param param { get; set; }
            }

            public class SearchRequestModel_req_1_param
            {
                public string num_per_page { get; set; }

                public string page_num { get; set; }

                public string query { get; set; }

                public int search_type { get; set; }
            }
        }


        internal class GetAlbumSongListRequestModel
        {
            public GetAlbumSongListRequestModel_comm comm { get; set; }

            public GetAlbumSongListRequestModel_albumSonglist albumSonglist { get; set; }

            internal class GetAlbumSongListRequestModel_comm
            {
                public int ct { get; set; }

                public int cv { get; set; }
            }

            internal class GetAlbumSongListRequestModel_albumSonglist
            {
                public string method { get; set; }

                public GetAlbumSongListRequestModel_albumSonglist_param param { get; set; }

                public string module { get; set; }
            }

            internal class GetAlbumSongListRequestModel_albumSonglist_param
            {
                public string albumMid { get; set; }

                public int albumID { get; set; }

                public int begin { get; set; }

                public int num { get; set; }

                public int order { get; set; }
            }
        }

        internal class GetSingerSongsRequestModel
        {
            public GetSingerSongsRequestModel_comm comm { get; set; }

            public GetSingerSongsRequestModel_singer singer { get; set; }

            internal class GetSingerSongsRequestModel_comm
            {
                public int ct { get; set; }

                public int cv { get; set; }
            }

            internal class GetSingerSongsRequestModel_singer
            {
                public string method { get; set; }

                public GetSingerSongsRequestModel_singer_param param { get; set; }

                public string module { get; set; }
            }

            internal class GetSingerSongsRequestModel_singer_param
            {
                public int sort { get; set; }

                public string singermid { get; set; }

                public int sin { get; set; }

                public int num { get; set; }
            }
        }

        internal class GetToplistRequestModel
        {
            public GetToplistRequestModel_comm comm { get; set; }

            public GetToplistRequestModel_detail detail { get; set; }

            public class GetToplistRequestModel_comm
            {
                public int ct { get; set; }

                public int cv { get; set; }
            }

            public class GetToplistRequestModel_detail
            {
                public string method { get; set; }

                public string module { get; set; }

                public GetToplistRequestModel_detail_param param { get; set; }
            }

            public class GetToplistRequestModel_detail_param
            {
                public int topId { get; set; }

                public int offset { get; set; }

                public int num { get; set; }

                public string period { get; set; }
            }
        }

        internal class GetSongLinkRequestModel
        {
            public GetSongLinkRequestModel_comm comm { get; set; }

            public GetSongLinkRequestModel_req req { get; set; }

            public GetSongLinkRequestModel_req0 req0 { get; set; }

            public class GetSongLinkRequestModel_comm
            {
                public int uin { get; set; }
                public string format { get; set; }
                public int ct { get; set; }
                public int cv { get; set; }
            }

            public class GetSongLinkRequestModel_req
            {
                public string method { get; set; }

                public string module { get; set; }

                public GetSongLinkRequestModel_req_param param { get; set; }
            }

            public class GetSongLinkRequestModel_req_param
            {
                public string guid { get; set; }

                public string calltype { get; set; }

                public string userip { get; set; }
            }

            public class GetSongLinkRequestModel_req0
            {
                public string method { get; set; }

                public string module { get; set; }

                public GetSongLinkRequestModel_req0_param param { get; set; }
            }

            public class GetSongLinkRequestModel_req0_param
            {
                public string guid { get; set; }

                public string[] songmid { get; set; }

                public int[] songtype { get; set; }

                public string uin { get; set; }

                public int loginflag { get; set; }

                public string platform { get; set; }
            }
        }

    }
}
