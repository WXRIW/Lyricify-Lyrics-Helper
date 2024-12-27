using System.Text.Json.Serialization;

#nullable disable
namespace Lyricify.Lyrics.Parsers.Models.Yrc
{
    public class CreditsInfo
    {
        [JsonPropertyName("t")]
        public int Timestamp { get; set; }

        [JsonPropertyName("c")]
        public List<Credit> Credits { get; set; }

        public class Credit
        {
            [JsonPropertyName("tx")]
            public string Text { get; set; }

            [JsonPropertyName("li")]
            public string Image { get; set; }

            [JsonPropertyName("or")]
            public string Orpheus { get; set; }
        }
    }

}
