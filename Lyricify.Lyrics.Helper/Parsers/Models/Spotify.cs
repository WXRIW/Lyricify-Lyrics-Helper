using System.Text.Json.Serialization;

#nullable disable
namespace Lyricify.Lyrics.Parsers.Models.Spotify
{
    public class SpotifyColorLyrics
    {
        [JsonPropertyName("lyrics")]
        public SpotifyLyrics Lyrics { get; set; }

        [JsonPropertyName("colors")]
        public SpotifyColors Colors { get; set; }

        [JsonPropertyName("hasVocalRemoval")]
        public bool HasVocalRemoval { get; set; }
    }

    public class SpotifyLyrics
    {
        [JsonPropertyName("syncType")]
        public string SyncType { get; set; }

        [JsonPropertyName("lines")]
        public List<SpotifyLyricsLine> Lines { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("providerLyricsId")]
        public string ProviderLyricsId { get; set; }

        [JsonPropertyName("providerDisplayName")]
        public string ProviderDisplayName { get; set; }

        [JsonPropertyName("syncLyricsUri")]
        public string SyncLyricsUri { get; set; }

        [JsonPropertyName("isDenseTypeface")]
        public bool IsDenseTypeface { get; set; }

        [JsonPropertyName("alternatives")]
        public List<AlternativeItem> Alternatives { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("isRtlLanguage")]
        public bool IsRtlLanguage { get; set; }

        [JsonPropertyName("fullscreenAction")]
        public string FullscreenAction { get; set; }
    }

    public class SpotifyLyricsLine
    {
        [JsonPropertyName("startTimeMs")]
        public string StartTimeMs { get; set; }

        public int StartTime
        {
            get
            {
                try
                {
                    return int.Parse(StartTimeMs);
                }
                catch
                {
                    return 0;
                }
            }
        }

        [JsonPropertyName("endTimeMs")]
        public string EndTimeMs { get; set; }

        public int EndTime
        {
            get
            {
                try
                {
                    return int.Parse(EndTimeMs);
                }
                catch
                {
                    return 0;
                }
            }
        }

        [JsonPropertyName("words")]
        public string Words { get; set; }

        [JsonPropertyName("syllables")]
        public List<SyllableItem> Syllables { get; set; }
    }

    public class SyllableItem
    {
        [JsonPropertyName("startTimeMs")]
        public string StartTimeMs { get; set; }

        public int StartTime
        {
            get
            {
                try
                {
                    return int.Parse(StartTimeMs);
                }
                catch
                {
                    return 0;
                }
            }
        }

        [JsonPropertyName("endTimeMs")]
        public string EndTimeMs { get; set; }

        public int EndTime
        {
            get
            {
                try
                {
                    return int.Parse(EndTimeMs);
                }
                catch
                {
                    return 0;
                }
            }
        }

        [JsonPropertyName("numChars")]
        public string NumberChars { get; set; }

        public int CharsCount
        {
            get
            {
                try
                {
                    return int.Parse(NumberChars);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }

    public class AlternativeItem
    {
        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("lines")]
        public List<string> Lines { get; set; }

        [JsonPropertyName("isRtlLanguage")]
        public bool IsRtlLanguage { get; set; }
    }

    public class SpotifyColors
    {
        [JsonPropertyName("background")]
        public int Background { get; set; }

        [JsonPropertyName("text")]
        public int Text { get; set; }

        [JsonPropertyName("highlightText")]
        public int HighlightText { get; set; }
    }
}
