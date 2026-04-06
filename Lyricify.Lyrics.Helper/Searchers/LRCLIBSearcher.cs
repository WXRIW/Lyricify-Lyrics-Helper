namespace Lyricify.Lyrics.Searchers
{
    public class LRCLIBSearcher : Searcher, ISearcher
    {
        public override string Name => "LRCLIB";

        public override string DisplayName => "LRCLIB";

        public override Searchers SearcherType => Searchers.LRCLIB;

        public override async Task<List<ISearchResult>?> SearchForResults(string searchString)
        {
            var search = new List<ISearchResult>();

            try
            {
                // 尝试从搜索字符串中智能提取艺人和曲目
                // searchString 格式通常是 "Title Artist Album"
                var parts = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return null;

                string trackName = searchString;
                string? artistName = null;

                // 如果有多个部分，尝试分离曲目和艺人
                if (parts.Length >= 2)
                {
                    // 简单策略：前半部分是曲目，后半部分是艺人
                    var midPoint = parts.Length / 2;
                    trackName = string.Join(" ", parts.Take(midPoint));
                    artistName = string.Join(" ", parts.Skip(midPoint));
                }

                // 先尝试带艺人名的搜索
                var results = await Providers.Web.Providers.LRCLIBApi.Search(trackName, artistName);

                // 如果没有结果，尝试只用曲目名搜索
                if (results == null || results.Count == 0)
                {
                    results = await Providers.Web.Providers.LRCLIBApi.Search(searchString);
                }

                if (results == null || results.Count == 0) return null;

                foreach (var track in results)
                {
                    search.Add(new LRCLIBSearchResult(track));
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
