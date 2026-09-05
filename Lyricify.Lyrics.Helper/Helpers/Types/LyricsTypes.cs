namespace Lyricify.Lyrics.Helpers.Types
{
    public static class Lrc
    {
        public static bool IsLrc(string input) => LyricsTypeDetector.IsLrc(input);
    }

    public static class LyricifyLines
    {
        public static bool IsLyricifyLines(string input) => LyricsTypeDetector.IsLyricifyLines(input);
    }

    public static class LyricifySyllable
    {
        public static bool IsLyricifySyllable(string input) => LyricsTypeDetector.IsLyricifySyllable(input);
    }

    public static class Qrc
    {
        public static bool IsQrc(string input) => LyricsTypeDetector.IsQrc(input);

        public static bool IsQrcFull(string input) => LyricsTypeDetector.IsQrcFull(input);
    }

    public static class Krc
    {
        public static bool IsKrc(string input) => LyricsTypeDetector.IsKrc(input);
    }

    public static class Yrc
    {
        public static bool IsYrc(string input) => LyricsTypeDetector.IsYrc(input);

        public static bool IsYrcFull(string input) => LyricsTypeDetector.IsYrcFull(input);
    }

    public static class Ttml
    {
        public static bool IsTtml(string input) => LyricsTypeDetector.IsTtml(input);
    }

    public static class AppleJson
    {
        public static bool IsAppleJson(string input) => LyricsTypeDetector.IsAppleJson(input);
    }

    public static class Spotify
    {
        public static bool IsSpotify(string input) => LyricsTypeDetector.IsSpotify(input);
    }

    public static class Musixmatch
    {
        public static bool IsMusixmatch(string input) => LyricsTypeDetector.IsMusixmatch(input);
    }
}
