namespace Lyricify.Lyrics.Searchers
{
    public class KugouSearcher : Searcher
    {
        public override string Name => "Kugou";

        public override string DisplayName => "Kugou Music";

        public override Task<List<ISearchResult>?> SearchForResults(string searchString)
        {
            throw new NotImplementedException();
        }
    }
}
