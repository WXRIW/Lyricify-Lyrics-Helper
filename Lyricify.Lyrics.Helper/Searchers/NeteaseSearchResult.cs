using Lyricify.Lyrics.Providers.Web.Netease;

namespace Lyricify.Lyrics.Searchers
{
    public class NeteaseSearchResult : SearchResult
    {
        public override ISearcher Searcher => new NeteaseSearcher();

        public NeteaseSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id) : base(title, artists, album, albumArtists, durationMs)
        {
            Id = id;
        }

        public NeteaseSearchResult(Song song) : this(
            song.Name,
            song.Artists.Select(s => s.Name).ToArray(),
            song.Album.Name,
            null,
            (int)song.Duration,
            int.Parse(song.Id)
            )
        { }

        public int Id { get; }
    }
}
