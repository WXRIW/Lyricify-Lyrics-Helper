using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Searchers.Helpers
{
    public static partial class CompareHelper
    {
        public static MatchType CompareTrack(ITrackMetadata track, ISearchResult searchResult)
        {
            return CompareTrack(TrackMultiArtistMetadata.GetTrackMultiArtistMetadata(track), searchResult);
        }

        public static MatchType CompareTrack(TrackMultiArtistMetadata track, ISearchResult searchResult)
        {
            var trackMatch = CompareName(track.Title, searchResult.Title);
            var artistMatch = CompareArtist(track.Artists, searchResult.Artists);
            var albumMatch = CompareName(track.Album, searchResult.Album);
            var albumArtistMatch = CompareArtist(track.AlbumArtists, searchResult.AlbumArtists);
            var durationMatch = CompareDuration(track.DurationMs, searchResult.DurationMs);

            var totalScore = 0d;
            totalScore += trackMatch.GetMatchScore();
            totalScore += artistMatch.GetMatchScore();
            totalScore += albumMatch.GetMatchScore() * 0.4;
            totalScore += albumArtistMatch.GetMatchScore() * 0.2;
            totalScore += durationMatch.GetMatchScore();

            return totalScore switch
            {
                > 21 => MatchType.Perfect,
                > 19 => MatchType.VeryHigh,
                > 17 => MatchType.High,
                > 15 => MatchType.PrettyHigh,
                > 11 => MatchType.Medium,
                > 8 => MatchType.Low,
                > 3 => MatchType.VeryLow,
                _ => MatchType.NoMatch,
            };
        }

        public enum MatchType
        {
            Perfect = 100,
            VeryHigh = 99,
            High = 95,
            PrettyHigh = 90,
            Medium = 70,
            Low = 30,
            VeryLow = 10,
            NoMatch = -1,
        }
    }

    public class MatchTypeComparer : IComparer<CompareHelper.MatchType>
    {
        public int Compare(CompareHelper.MatchType x, CompareHelper.MatchType y)
        {
            return ((int)x).CompareTo((int)y);
        }
    }
}
