using Lyricify.Lyrics.Models;
using Lyricify.Lyrics.Searchers.Helpers;
using System.Linq;

namespace Lyricify.Lyrics.Searchers
{
    public class SpotifySearcher : ISearcher
    {
        public string Name => "Spotify";

        public string DisplayName => "Spotify";

        public Searchers SearcherType => Searchers.Spotify;

        public async Task<ISearchResult?> SearchForResult(ITrackMetadata track)
        {
            var results = await SearchForResults(track);
            return results.FirstOrDefault();
        }

        public async Task<ISearchResult?> SearchForResult(ITrackMetadata track, CompareHelper.MatchType minimumMatch)
        {
            var results = await SearchForResults(track);
            return results.FirstOrDefault(t => t.MatchType >= minimumMatch);
        }

        public async Task<List<ISearchResult>> SearchForResults(ITrackMetadata track)
        {
            return await SearchForResults(track, false);
        }

        public async Task<List<ISearchResult>> SearchForResults(ITrackMetadata track, bool fullSearch)
        {
            try
            {
                var metadata = TrackMultiArtistMetadata.GetTrackMultiArtistMetadata(track);
                var song = metadata.Title ?? string.Empty;
                var artist = metadata.Artist ?? string.Empty;
                var candidates = await Providers.Web.Providers.SpotifyApi.SearchTrackCandidates(song, artist, 20);

                var results = candidates.Select(t => (ISearchResult)new SpotifySearchResult(t)).ToList();
                foreach (var result in results)
                {
                    result.SetMatchType(CompareHelper.CompareTrack(metadata, result));
                }

                results.Sort((x, y) => -((int)x.MatchType!).CompareTo((int)y.MatchType!));
                return results;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch
            {
                return new List<ISearchResult>();
            }
        }

        public async Task<List<ISearchResult>?> SearchForResults(string searchString)
        {
            try
            {
                var candidates = await Providers.Web.Providers.SpotifyApi.SearchTrackCandidates(searchString, string.Empty, 20);
                if (candidates is not { Count: > 0 }) return null;

                return candidates.Select(t => (ISearchResult)new SpotifySearchResult(t)).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }
    }
}
