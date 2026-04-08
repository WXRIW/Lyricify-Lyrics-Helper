using Lyricify.Lyrics.Providers.Web.Spotify;
using Lyricify.Lyrics.Searchers.Helpers;
using System.Linq;

namespace Lyricify.Lyrics.Searchers
{
    public class SpotifySearchResult : ISearchResult
    {
        public ISearcher Searcher => new SpotifySearcher();

        public SpotifySearchResult(string title, string[] artists, string album, string[]? albumArtists, int? durationMs, string id)
        {
            Title = title;
            Artists = artists;
            Album = album;
            AlbumArtists = albumArtists;
            DurationMs = durationMs;
            Id = id;
        }

        public SpotifySearchResult(SearchTrackItem track) : this(
            track.Name ?? string.Empty,
            track.Artists?.Select(t => t.Name).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray() ?? System.Array.Empty<string>(),
            track.Album?.Name ?? string.Empty,
            track.Album?.Artists?.Select(t => t.Name).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray(),
            track.DurationMs,
            track.Id ?? string.Empty)
        {
        }

        public SpotifySearchResult(SpotifyTrackCandidate track) : this(
            track.Title ?? string.Empty,
            string.IsNullOrWhiteSpace(track.ArtistName) ? Array.Empty<string>() : track.ArtistName.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToChineselizeArtists(),
            track.AlbumName ?? string.Empty,
            null,
            track.DurationMs ?? 0,
            track.Id ?? string.Empty)
        {
        }

        public string Title { get; }

        public string[] Artists { get; }

        public string Album { get; }

        public string Id { get; }

        public string[]? AlbumArtists { get; }

        public int? DurationMs { get; }

        public CompareHelper.MatchType? MatchType { get; set; }
    }
}
