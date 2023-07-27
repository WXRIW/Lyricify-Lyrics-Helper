using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Helpers
{
    public static class ParseHelper
    {
        public static LyricsData? ParseLyrics(string lyrics)
        {
            var type = TypeHelper.GetLyricsTypes(lyrics);
            if (type != LyricsRawTypes.Unknown)
            {
                ParseLyrics(lyrics, type);
            }
            return null;
        }

        public static LyricsData? ParseLyrics(string lyrics, LyricsRawTypes lyricsRawType)
        {
            switch (lyricsRawType)
            {
                case LyricsRawTypes.LyricifyLines:
                    return Parsers.LyricifyLinesParser.Parse(lyrics);
                case LyricsRawTypes.Lrc:
                    return Parsers.LrcParser.Parse(lyrics);
                case LyricsRawTypes.Krc:
                    return Parsers.KrcParser.Parse(lyrics);
                case LyricsRawTypes.Yrc:
                    return Parsers.YrcParser.Parse(lyrics);
                case LyricsRawTypes.Spotify:
                    return Parsers.SpotifyParser.Parse(lyrics);
            }
            return null;
        }
    }
}
