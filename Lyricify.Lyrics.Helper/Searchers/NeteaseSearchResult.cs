using Lyricify.Lyrics.Providers.Web.Netease;
using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public class NeteaseSearchResult : ISearchResult
    {
        public ISearcher Searcher => new NeteaseSearcher();

        public NeteaseSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id)
        {
            Title = title;
            Artists = artists;
            Album = album;
            AlbumArtists = albumArtists;
            DurationMs = durationMs;
            Id = id;
        }

        public NeteaseSearchResult(Song song) : this(
            song.Name,
            song.Ar.Select(s => s.Name).ToArray(),
            song.Al.Name,
            null,
            (int)song.Dt,
            int.Parse(song.Id)
            )
        { }

        public string Title { get; }

        public string[] Artists { get; }

        public string Album { get; }

        public int Id { get; }

        public string[]? AlbumArtists { get; }

        public int? DurationMs { get; }

        public CompareHelper.MatchType? MatchType { get; set; }
    }
}
