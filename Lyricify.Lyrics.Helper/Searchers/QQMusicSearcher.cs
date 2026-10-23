using Lyricify.Lyrics.Helpers.General;
using Lyricify.Lyrics.Models;
using Lyricify.Lyrics.Providers.Web.QQMusic;
using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public class QQMusicSearcher : Searcher, ISearcher
    {
        public override string Name => "QQ Music";

        public override string DisplayName => "QQ Music";

        public override Searchers SearcherType => Searchers.QQMusic;

        public new Task<ISearchResult?> SearchForResult(ITrackMetadata track)
            => SearchTrack(track);

        public new Task<ISearchResult?> SearchForResult(
            ITrackMetadata track,
            CompareHelper.MatchType minimumMatch)
            => SearchTrack(track, minimumMatch);

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

        private async Task<ISearchResult?> SearchTrack(
            ITrackMetadata track,
            CompareHelper.MatchType? minimumMatch = null)
        {
            ISearchResult? best = null;

            foreach (var query in BuildSearchQueries(track))
            {
                var search = await SearchForResults(query);
                if (search is not { Count: > 0 })
                    continue;

                foreach (var result in search)
                    result.SetMatchType(CompareHelper.CompareTrack(track, result));
                var candidate = search.OrderByDescending(result => (int)result.MatchType!).First();

                if (best is null || (int)candidate.MatchType! > (int)best.MatchType!)
                    best = candidate;
                if (minimumMatch is null || (int)best.MatchType! >= (int)minimumMatch)
                    break;
            }

            return minimumMatch is null || best is not null
                && (int)best.MatchType! >= (int)minimumMatch
                ? best
                : null;
        }

        private static IReadOnlyList<string> BuildSearchQueries(ITrackMetadata track)
        {
            var title = track.Title?.Trim() ?? string.Empty;
            var artist = track.Artist?.Trim().Replace(", ", " ") ?? string.Empty;
            var album = track.Album?.Trim() ?? string.Empty;
            var firstArtist = string.IsNullOrWhiteSpace(track.Artist)
                ? null
                : track.Artist.Split(
                    new[] { ',', '/', '／', '、', ';', '；', '&', '＆' },
                    StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return new[]
            {
                Join(title, artist, album),
                Join(title, artist),
                Join(title),
                Join(Normalize(title), Normalize(artist), Normalize(album)),
                Join(RemoveSearchSuffix(title), artist, RemoveSearchSuffix(album)),
                Join(RemoveSearchSuffix(title), artist),
                Join(title, firstArtist, album),
            }
            .Where(query => query.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        }

        private static string RemoveSearchSuffix(string value)
        {
            var markers = new[] { "(feat.", "(ft.", "(featuring", " feat.", " ft." };
            foreach (var marker in markers)
            {
                var index = value.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (index > 0)
                    return value[..index].TrimEnd();
            }

            foreach (var pair in new[] { ('(', ')'), ('[', ']'), ('（', '）'), ('【', '】') })
            {
                var index = value.LastIndexOf(pair.Item1);
                if (value.EndsWith(pair.Item2) && index > 0)
                    return value[..index].TrimEnd();
            }

            return value;
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var chars = value.ToSC(true).ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] is ',' or '，' or '/' or '／' or '、' or ';' or '；' or '&' or '＆'
                    or '-' or '‐' or '–' or '—' or ':' or '：' or '（' or '）')
                {
                    chars[i] = ' ';
                }
            }

            return new string(chars).RemoveDuoSpaces().Trim();
        }

        private static string Join(params string?[] parts)
        {
            return string.Join(" ", parts.Where(part => !string.IsNullOrWhiteSpace(part)))
                .RemoveDuoSpaces()
                .Trim();
        }
    }
}
