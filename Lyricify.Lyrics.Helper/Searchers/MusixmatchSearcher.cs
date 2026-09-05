using Lyricify.Lyrics.Models;
using Lyricify.Lyrics.Searchers.Helpers;

namespace Lyricify.Lyrics.Searchers
{
    public class MusixmatchSearcher : ISearcher
    {
        public string Name => "Musixmatch";

        public string DisplayName => "Musixmatch";

        public Searchers SearcherType => Searchers.Musixmatch;

        public async Task<ISearchResult?> SearchForResult(ITrackMetadata track)
        {
            var result = await SearchForResults(track);
            if (result is { Count: > 0 })
                return result[0];
            return null;
        }

        public async Task<ISearchResult?> SearchForResult(ITrackMetadata track, CompareHelper.MatchType minimumMatch)
        {
            var result = await SearchForResults(track);
            if (result is { Count: > 0 } && (int)result[0].MatchType! >= (int)minimumMatch)
                return result[0];
            return null;
        }

        public async Task<List<ISearchResult>?> SearchForResults(string track, string artist, int? duration = null)
        {
            var search = new List<ISearchResult>();

            try
            {
                var result = await Providers.Web.Providers.MusixmatchApi.GetTrack(track, artist, duration / 1000);
                var t = result?.Message?.Body?.Track;
                if (t == null) return null;
                var r = new MusixmatchSearchResult(t)
                {
                    MatchType = result!.Message.Header.Confidence switch
                    {
                        1000 => CompareHelper.MatchType.Perfect,
                        >= 950 => CompareHelper.MatchType.VeryHigh,
                        >= 900 => CompareHelper.MatchType.High,
                        >= 750 => CompareHelper.MatchType.PrettyHigh,
                        >= 600 => CompareHelper.MatchType.Medium,
                        >= 400 => CompareHelper.MatchType.Low,
                        >= 200 => CompareHelper.MatchType.VeryLow,
                        _ => CompareHelper.MatchType.NoMatch,
                    }
                };
                search.Add(r);
            }
            catch
            {
                return null;
            }

            return search;
        }

        public async Task<List<ISearchResult>> SearchForResults(ITrackMetadata track)
        {
            return await SearchForResults(track.Title!, track.Artist!, track.DurationMs) ?? new();
        }

        public async Task<List<ISearchResult>> SearchForResults(ITrackMetadata track, bool fullSearch)
        {
            return await SearchForResults(track);
        }

        public async Task<List<ISearchResult>?> SearchForResults(string searchString)
        {
            return await SearchForResultsAsync(
                searchString,
                null,
                null,
                null,
                CancellationToken.None);
        }

        public async Task<List<ISearchResult>> SearchForResultsAsync(
            string? keyword,
            string? track,
            string? artist,
            int? durationMs,
            CancellationToken cancellationToken)
        {
            var tracks = await Providers.Web.Providers.MusixmatchApi.SearchTracksAsync(
                keyword,
                track,
                artist,
                durationMs is > 0 ? durationMs / 1000 : null,
                cancellationToken);
            var results = tracks
                .Select(result => (ISearchResult)new MusixmatchSearchResult(result))
                .ToList();
            if (string.IsNullOrWhiteSpace(track))
            {
                return results;
            }

            var metadata = new TrackMetadata
            {
                Title = track,
                Artist = artist,
                DurationMs = durationMs,
            };
            foreach (var result in results)
            {
                result.SetMatchType(CompareHelper.CompareTrack(metadata, result));
            }
            results.Sort((left, right) =>
                -((int)left.MatchType!).CompareTo((int)right.MatchType!));
            return results;
        }
    }
}
