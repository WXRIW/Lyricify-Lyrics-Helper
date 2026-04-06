using Newtonsoft.Json;

namespace Lyricify.Lyrics.Providers.Web.LRCLIB
{
    public class SearchResultItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("trackName")]
        public string TrackName { get; set; } = string.Empty;

        [JsonProperty("artistName")]
        public string ArtistName { get; set; } = string.Empty;

        [JsonProperty("albumName")]
        public string AlbumName { get; set; } = string.Empty;

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("instrumental")]
        public bool Instrumental { get; set; }

        [JsonProperty("plainLyrics")]
        public string? PlainLyrics { get; set; }

        [JsonProperty("syncedLyrics")]
        public string? SyncedLyrics { get; set; }
    }

    public class GetLyricResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("trackName")]
        public string TrackName { get; set; } = string.Empty;

        [JsonProperty("artistName")]
        public string ArtistName { get; set; } = string.Empty;

        [JsonProperty("albumName")]
        public string AlbumName { get; set; } = string.Empty;

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("instrumental")]
        public bool Instrumental { get; set; }

        [JsonProperty("plainLyrics")]
        public string? PlainLyrics { get; set; }

        [JsonProperty("syncedLyrics")]
        public string? SyncedLyrics { get; set; }
    }
}
