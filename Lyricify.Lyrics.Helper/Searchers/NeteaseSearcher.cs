using Lyricify.Lyrics.Providers.Web.Netease;

namespace Lyricify.Lyrics.Searchers
{
    public class NeteaseSearcher : Searcher, ISearcher
    {
        public override string Name => "Netease";

        public override string DisplayName => "Netease Cloud Music";

        public override async Task<List<ISearchResult>?> SearchForResults(string searchString)
        {
            var search = new List<ISearchResult>();

            try
            {
                var result = await Providers.Web.Providers.NeteaseApi.Search(searchString, Api.SearchTypeEnum.SONG_ID);
                var results = result?.Result.Songs;
                if (results == null) return null;
                foreach (var track in results)
                {
                    search.Add(new NeteaseSearchResult(track));
                }
            }
            catch
            {
                return null;
            }

            return search;
        }
    }
}
