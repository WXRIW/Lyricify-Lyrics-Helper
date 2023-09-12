using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public interface ISearchResult
    {
        public ISearcher Searcher { get; }

        public string Title { get; }

        public string[] Artists { get; }

        public string Artist => string.Join(", ", Artists);

        public string Album { get; }

        public string[]? AlbumArtists { get; }

        public string? AlbumArtist => string.Join(", ", AlbumArtists ?? new string[0]);

        public int? DurationMs { get; }

        public CompareHelper.MatchType? MatchType { get; protected set; }

        public void SetMatchType(CompareHelper.MatchType? matchType)
        {
            MatchType = matchType;
        }
    }
}
