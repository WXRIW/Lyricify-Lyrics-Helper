using Lyricify.Lyrics.Providers.Web.QQMusic;

namespace Lyricify.Lyrics.Searchers
{
    public class QQMusicSearcher : Searcher, ISearcher
    {
        public override string Name => "QQ Music";

        public override string DisplayName => "QQ Music";

        public override Searchers SearcherType => Searchers.QQMusic;

        public override async Task<List<ISearchResult>?> SearchForResults(string searchString)
        {
            try
            {
                var result = await Providers.Web.Providers.QQMusicApi.SearchMobile(searchString);
                var search = GetSearchResults(result);
                if (search is { Count: > 0 })
                    return search;
            }
            catch
            {
                // Mobile 请求、解析或结果构造失败时，继续尝试 Desktop。
            }

            try
            {
                // 同一关键词仅回退一次，其他关键词仍由基类处理。
                var result = await Providers.Web.Providers.QQMusicApi.Search(searchString, Api.SearchTypeEnum.SONG_ID);
                return GetSearchResults(result);
            }
            catch
            {
                return null;
            }
        }

        private static List<ISearchResult>? GetSearchResults(MusicFcgApiResult? result)
        {
            if (result?.Code != 0 || result.Req_1?.Code != 0 || result.Req_1.Data?.Code != 0)
                return null;
            var results = result.Req_1.Data.Body?.Song?.List;
            if (results == null) return null;

            var search = new List<ISearchResult>();
            foreach (var track in results)
            {
                if (string.IsNullOrWhiteSpace(track.Id) || string.IsNullOrWhiteSpace(track.Mid))
                    return null;
                search.Add(new QQMusicSearchResult(track));
                if (track.Group is { Count: > 0 } group)
                {
                    foreach (var subTrack in group)
                    {
                        if (string.IsNullOrWhiteSpace(subTrack.Id) || string.IsNullOrWhiteSpace(subTrack.Mid))
                            return null;
                        search.Add(new QQMusicSearchResult(subTrack));
                    }
                }
            }
            return search;
        }
    }
}
