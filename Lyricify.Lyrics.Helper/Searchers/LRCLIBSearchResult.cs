using Lyricify.Lyrics.Providers.Web.LRCLIB;
using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public class LRCLIBSearchResult : ISearchResult
    {
        public ISearcher Searcher => new LRCLIBSearcher();

        public LRCLIBSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id)
        {
            Title = title;
            Artists = artists;
            Album = album;
            AlbumArtists = albumArtists;
            DurationMs = durationMs;
            Id = id;
        }

        public LRCLIBSearchResult(SearchResultItem item) : this(
            item.TrackName,
            item.ArtistName.Split(new string[] { ", ", " & ", " feat. ", " ft. " }, StringSplitOptions.RemoveEmptyEntries),
            item.AlbumName,
            null,
            (int)(item.Duration * 1000),
            item.Id
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
