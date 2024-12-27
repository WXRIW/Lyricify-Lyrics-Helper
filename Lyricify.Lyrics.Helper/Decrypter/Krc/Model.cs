using System.Text.Json.Serialization;

namespace Lyricify.Lyrics.Decrypter.Krc
{
    public class KugouLyricsResponse
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("info")]
        public string? Info { get; set; }

        [JsonPropertyName("_source")]
        public string? Source { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("contenttype")]
        public int ContentType { get; set; }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("fmt")]
        public string? Format { get; set; }
    }

    public class KugouTranslation
    {
        [JsonPropertyName("content")]
        public List<ContentItem>? Content { get; set; }

        public class ContentItem
        {
            [JsonPropertyName("language")]
            public int Language { get; set; }

            [JsonPropertyName("type")]
            public int Type { get; set; }

            [JsonPropertyName("lyricContent")]
            public List<List<string>?>? LyricContent { get; set; }
        }

        [JsonPropertyName("version")]
        public int Version { get; set; }
    }
}