using System.Text.Json.Serialization;

#nullable disable
namespace Lyricify.Lyrics.Parsers.Models
{
    public class RichSyncedLine
    {
        [JsonPropertyName("ts")]
        public float TimeStart { get; set; }

        [JsonPropertyName("te")]
        public float TimeEnd { get; set; }

        [JsonPropertyName("l")]
        public List<Word> Words { get; set; }

        public class Word
        {
            [JsonPropertyName("c")]
            public string Chars { get; set; }

            [JsonPropertyName("o")]
            public double Position { get; set; }
        }

        [JsonPropertyName("x")]
        public string Text { get; set; }
    }
}
