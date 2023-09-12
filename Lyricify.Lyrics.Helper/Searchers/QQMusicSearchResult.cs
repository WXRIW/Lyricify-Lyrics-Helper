using Lyricify.Lyrics.Providers.Web.QQMusic;
using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public class QQMusicSearchResult : ISearchResult
    {
        public ISearcher Searcher => new QQMusicSearcher();

        public QQMusicSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id, string mid)
        {
            Title = title;
            Artists = artists;
            Album = album;
            AlbumArtists = albumArtists;
            DurationMs = durationMs;
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

        public string Title { get; }

        public string[] Artists { get; }

        public string Album { get; }

        public int Id { get; }

        public string Mid { get; }

        public string[]? AlbumArtists { get; }

        public int? DurationMs { get; }

        public CompareHelper.MatchType? MatchType { get; set; }
    }
}
