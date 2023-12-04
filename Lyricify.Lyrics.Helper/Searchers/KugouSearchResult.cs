using Lyricify.Lyrics.Providers.Web.Kugou;

namespace Lyricify.Lyrics.Searchers
{
    public class KugouSearchResult : SearchResult
    {
        public override ISearcher Searcher => new KugouSearcher();

        public KugouSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, string hash) : base(title, artists, album, albumArtists, durationMs)
        {
            Hash = hash;
        }

        public KugouSearchResult(SearchSongResponse.DataItem.InfoItem song) : this(
            song.SongName,
            song.SingerName.Split('、'),
            song.AlbumName, // 很可能会包含中文译名
            null,
            song.Duration * 1000,
            song.Hash
            )
        { }

        public string Hash { get; }
    }
}
