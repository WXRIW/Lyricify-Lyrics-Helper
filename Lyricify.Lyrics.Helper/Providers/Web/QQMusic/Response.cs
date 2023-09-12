using Newtonsoft.Json;
using System.Text;

#nullable disable
namespace Lyricify.Lyrics.Providers.Web.QQMusic
{
    public class MusicFcgApiResult
    {
        public long Code { get; set; }

        public long Ts { get; set; }

        public long StartTs { get; set; }

        public string Traceid { get; set; }

        public MusicFcgReq Req { get; set; }

        public MusicFcgReq0 Req_0 { get; set; }

        public MusicFcgReq1 Req_1 { get; set; }

        public class MusicFcgReq
        {
            public long Code { get; set; }

            public MusicFcgReqData Data { get; set; }

            public class MusicFcgReqData
            {
                public string[] Sip { get; set; }

                public string Keepalivefile { get; set; }

                public string Testfile2g { get; set; }

                public string Testfilewifi { get; set; }
            }
        }

        public class MusicFcgReq0
        {
            public long Code { get; set; }

            public MusicFcgReq0Data Data { get; set; }

            public class MusicFcgReq0Data
            {
                public string[] Sip { get; set; }

                public string Testfile2g { get; set; }

                public string Testfilewifi { get; set; }

                public MusicFcgReq0DataMidurlinfo[] Midurlinfo { get; set; }
            }

            public class MusicFcgReq0DataMidurlinfo
            {
                public string Songmid { get; set; }

                public string Purl { get; set; }
            }
        }

        public class MusicFcgReq1
        {
            public long Code { get; set; }

            public MusicFcgReq1Data Data { get; set; }

            public class MusicFcgReq1Data
            {
                public long Code { get; set; }

                public long Ver { get; set; }

                public MusicFcgReq1DataBody Body { get; set; }

                public MusicFcgReq1DataMeta Meta { get; set; }

                public class MusicFcgReq1DataBody
                {
                    /// <summary>
                    /// 专辑查询结果
                    /// </summary>
                    public MusicFcgReq1DataBodyAlbum Album { get; set; }

                    /// <summary>
                    /// 单曲查询结果
                    /// </summary>
                    public MusicFcgReq1DataBodySong Song { get; set; }

                    /// <summary>
                    /// 歌单查询结果
                    /// </summary>
                    public MusicFcgReq1DataBodyPlayList Songlist { get; set; }

                    public class MusicFcgReq1DataBodyAlbum
                    {
                        public MusicFcgReq1DataBodyAlbumInfo[] List { get; set; }

                        public class MusicFcgReq1DataBodyAlbumInfo
                        {
                            public long AlbumID { get; set; }

                            public string AlbumMID { get; set; }

                            /// <summary>
                            /// 专辑名
                            /// </summary>
                            public string AlbumName { get; set; }

                            public long Song_count { get; set; }

                            /// <summary>
                            /// 发布时间 eg 2011-04-27
                            /// </summary>
                            public string PublicTime { get; set; }

                            public Singer[] Singer_list { get; set; }
                        }
                    }

                    public class MusicFcgReq1DataBodySong
                    {
                        public Song[] List { get; set; }
                    }

                    public class MusicFcgReq1DataBodyPlayList
                    {
                        public MusicFcgReq1DataBodyPlayListInfo[] List { get; set; }

                        public class MusicFcgReq1DataBodyPlayListInfo
                        {
                            /// <summary>
                            /// 歌单 ID
                            /// </summary>
                            public string Dissid { get; set; }

                            /// <summary>
                            /// 歌单名
                            /// </summary>
                            public string Dissname { get; set; }

                            /// <summary>
                            /// 歌单封面
                            /// </summary>
                            public string Imgurl { get; set; }

                            /// <summary>
                            /// 歌单介绍
                            /// </summary>
                            public string Introduction { get; set; }

                            /// <summary>
                            /// 歌单数量
                            /// </summary>
                            public long Song_Count { get; set; }

                            /// <summary>
                            /// 歌单播放量
                            /// </summary>
                            public long Listennum { get; set; }

                            public MusicFcgReq1DataBodyPlayListCreator Creator { get; set; }
                        }

                        public class MusicFcgReq1DataBodyPlayListCreator
                        {
                            public string Name { get; set; }

                            public long Qq { get; set; }
                        }
                    }
                }

                public class MusicFcgReq1DataMeta
                {
                    /// <summary>
                    /// 当前页数
                    /// </summary>
                    public long Curpage { get; set; }

                    /// <summary>
                    /// 下一页
                    /// </summary>
                    public long Nextpage { get; set; }

                    /// <summary>
                    /// 每页数量
                    /// </summary>
                    public long Perpage { get; set; }

                    /// <summary>
                    /// 查询关键字
                    /// </summary>
                    public string Query { get; set; }

                    /// <summary>
                    /// 累计数量
                    /// </summary>
                    public long Sum { get; set; }
                }
            }
        }
    }

    public class MusicFcgApiAlternativeResult
    {
        public long Code { get; set; }

        public long Ts { get; set; }

        public long StartTs { get; set; }

        public string Traceid { get; set; }

        [JsonProperty("music.search.SearchCgiService")]
        public SearchCgiService Search { get; set; }

        public class SearchCgiService
        {
            public DataBody Data { get; set; }
        }

        public class DataBody
        {
            public SearchData Body { get; set; }
        }

        public class SearchData
        {
            public Song Song { get; set; }
        }

        public class Song
        {
            public QQMusic.Song[] List { get; set; }
        }
    }

    /// <summary>
    /// 专辑查询接口返回结果
    /// </summary>
    public class AlbumResult
    {
        public long Code { get; set; }

        public AlbumInfo Data { get; set; }

        public string Message { get; set; }
    }

    /// <summary>
    /// 歌单查询接口返回结果
    /// </summary>
    public class PlaylistResult
    {
        public long Code { get; set; }

        public Playlist[] Cdlist { get; set; }
    }

    /// <summary>
    /// 歌曲查询接口返回结果
    /// </summary>
    public class SongResult
    {
        public long Code { get; set; }

        public Song[] Data { get; set; }

        public bool IsIllegal()
        {
            return Code != 0 || Data.Length == 0;
        }
    }

    /// <summary>
    /// 歌词查询接口返回结果
    /// </summary>
    public class LyricResult
    {
        public long Code { get; set; }

        public string Lyric { get; set; }

        public string Trans { get; set; }

        public LyricResult Decode()
        {
            if (Lyric != null)
            {
                Lyric = Encoding.UTF8.GetString(Convert.FromBase64String(Lyric));
            }

            if (Trans != null)
            {
                Trans = Encoding.UTF8.GetString(Convert.FromBase64String(Trans));
            }

            return this;
        }
    }

    public class Playlist
    {
        /// <summary>
        /// 歌单 ID
        /// </summary>
        public string Disstid { get; set; }

        /// <summary>
        /// 歌单名
        /// </summary>
        public string Dissname { get; set; }

        /// <summary>
        /// 作者名
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 歌单封面
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 歌单描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public Tag[] Tags { get; set; }

        /// <summary>
        /// 歌曲数量
        /// </summary>
        public long Songnum { get; set; }

        /// <summary>
        /// 歌曲 ID 列表，使用英文逗号分割
        /// </summary>
        public string Songids { get; set; }

        /// <summary>
        /// 歌曲列表
        /// </summary>
        public Song[] SongList { get; set; }

        /// <summary>
        /// 播放量
        /// </summary>
        public long Visitnum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CTime { get; set; }
    }

    public class Tag
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long Pid { get; set; }
    }

    public class Song
    {
        /// <summary>
        /// 所属专辑
        /// </summary>
        public SongAlbum Album { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// 时长，单位 s
        /// </summary>
        public int Interval { get; set; }

        public string Mid { get; set; }

        /// <summary>
        /// 歌曲名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 歌曲描述
        /// </summary>
        public string Desc { get; set; }

        public Singer[] Singer { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发布时间，eg: 2005-07-08
        /// </summary>
        public string Time_public { get; set; }

        public class SongAlbum
        {
            public long Id { get; set; }

            public string Mid { get; set; }

            public string Pmid { get; set; }

            public string Name { get; set; }

            public string Title { get; set; }

            public string Subtitle { get; set; }
        }
    }

    public class AlbumInfo
    {
        /// <summary>
        /// 专辑时间，eg 2022-03-28
        /// </summary>
        public string ADate { get; set; }

        /// <summary>
        /// 发行公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 专辑描述
        /// </summary>
        public string Desc { get; set; }

        public long Id { get; set; }

        public string Mid { get; set; }

        /// <summary>
        /// 专辑语言，eg 韩语
        /// </summary>
        public string Lan { get; set; }

        public AlbumSong[] List { get; set; }

        /// <summary>
        /// 专辑名
        /// </summary>
        public string Name { get; set; }

        public long Singerid { get; set; }

        public string Singermid { get; set; }

        public string Singername { get; set; }

        /// <summary>
        /// 包含的歌曲数量
        /// </summary>
        public int Total { get; set; }

        public class AlbumSong
        {
            public Singer[] Singer { get; set; }

            public long Songid { get; set; }

            public string Songmid { get; set; }

            public string Songname { get; set; }
        }
    }

    public class Singer
    {
        public long Id { get; set; }

        public string Mid { get; set; }

        public string Name { get; set; }

        public string Pmid { get; set; }

        public string Title { get; set; }

        public int Type { get; set; }

        public long Uin { get; set; }
    }
}
