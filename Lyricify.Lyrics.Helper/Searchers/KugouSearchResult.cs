using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public class KugouSearchResult : ISearchResult
    {
        public ISearcher Searcher => throw new NotImplementedException();

        public string Title => throw new NotImplementedException();

        public string[] Artists => throw new NotImplementedException();

        public string Album => throw new NotImplementedException();

        public string[]? AlbumArtists => throw new NotImplementedException();

        public int? DurationMs => throw new NotImplementedException();

        public CompareHelper.MatchType? MatchType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
