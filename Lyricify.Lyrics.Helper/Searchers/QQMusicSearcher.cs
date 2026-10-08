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
            var search = new List<ISearchResult>();

            try
            {
                var result = await Providers.Web.Providers.QQMusicApi.Search(searchString, Api.SearchTypeEnum.SONG_ID);
                var results = result?.Req_1?.Data?.Body?.Song?.List;
                if (results == null) return null;
                foreach (var track in results)
                {
                    search.Add(new QQMusicSearchResult(track));
                    if (track.Group is { Count: > 0 } group)
                    {
                        foreach (var subTrack in group)
                        {
                            search.Add(new QQMusicSearchResult(subTrack));
                        }
                    }
                }
            }
            catch
            {
                return null;
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
