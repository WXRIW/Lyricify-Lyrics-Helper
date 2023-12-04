using Lyricify.Lyrics.Providers.Web.QQMusic;

namespace Lyricify.Lyrics.Searchers
{
    public class QQMusicSearchResult : SearchResult
    {
        public override ISearcher Searcher => new QQMusicSearcher();

        public QQMusicSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id, string mid) : base(title, artists, album, albumArtists, durationMs)
        {
            Id = id;
            Mid = mid;
        }

        public QQMusicSearchResult(Song song) : this(
            song.Title,
            song.Singer.Select(s => s.Title).ToArray(),
            song.Album.Title,
            null,
            song.Interval * 1000,
            int.Parse(song.Id),
            song.Mid
            )
        { }

        public int Id { get; }

        public string Mid { get; }
    }
}
