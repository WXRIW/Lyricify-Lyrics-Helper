using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public abstract class SearchResult : ISearchResult
    {
        public SearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs)
        {
            Title = title;
            Artists = artists;
            Album = album;
            AlbumArtists = albumArtists;
            DurationMs = durationMs;
        }

        /// <summary>
        /// 搜索提供者
        /// </summary>
        public abstract ISearcher Searcher { get; }

        /// <summary>
        /// 曲目名
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 艺人列表
        /// </summary>
        public string[] Artists { get; }

        /// <summary>
        /// 艺人名
        /// </summary>
        public string Artist => string.Join(", ", Artists);

        /// <summary>
        /// 专辑
        /// </summary>
        public string Album { get; }

        /// <summary>
        /// 专辑艺人列表
        /// </summary>
        public string[]? AlbumArtists { get; }

        /// <summary>
        /// 专辑艺人名
        /// </summary>
        public string? AlbumArtist => string.Join(", ", AlbumArtists ?? new string[0]);

        /// <summary>
        /// 曲目时长
        /// </summary>
        public int? DurationMs { get; }

        /// <summary>
        /// 匹配程度
        /// </summary>
        public CompareHelper.MatchType? MatchType { get; set; }

        /// <summary>
        /// 设置匹配程度
        /// </summary>
        /// <param name="matchType"></param>
        public void SetMatchType(CompareHelper.MatchType? matchType)
        {
            MatchType = matchType;
        }
    }
}
