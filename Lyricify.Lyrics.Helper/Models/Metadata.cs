namespace Lyricify.Lyrics.Models
{
    public class TrackMetadata : ITrackMetadata
    {
        public string? Title { get; set; }

        public string? Artist { get; set; }

        public string? Album { get; set; }

        public string? AlbumArtist { get; set; }

        public string? DurationMs { get; set; }

        public string? Isrc { get; set; }

        public List<string>? Language { get; set; }
    }

    public class SpotifyTrackMetadata : TrackMetadata, ITrackMetadata
    {
        /// <summary>
        /// Spotify ID of the track
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Spotify URI of the track
        /// </summary>
        public string? Uri { get; set; }
    }
}
