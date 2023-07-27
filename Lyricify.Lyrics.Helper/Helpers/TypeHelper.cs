using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Helpers
{
    public static class TypeHelper
    {
        /// <summary>
        /// Recognize the type of lyrics
        /// </summary>
        /// <param name="lyrics">Lyrics string</param>
        /// <returns><see cref="LyricsRawTypes"/>, <see cref="LyricsRawTypes.Unknown"/> if not recognized.</returns>
        public static LyricsRawTypes GetLyricsTypes(string lyrics)
        {
            return LyricsRawTypes.Unknown;
        }

        /// <summary>
        /// Convert LyricsRawType into LyricsType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static LyricsTypes GetLyricsType(this LyricsRawTypes type) => type switch
        {
            LyricsRawTypes.Unknown => LyricsTypes.Unknown,
            LyricsRawTypes.LyricifySyllable => LyricsTypes.LyricifySyllable,
            LyricsRawTypes.LyricifyLines => LyricsTypes.LyricifyLines,
            LyricsRawTypes.Lrc => LyricsTypes.Lrc,
            LyricsRawTypes.Qrc => LyricsTypes.Qrc,
            LyricsRawTypes.QrcFull => LyricsTypes.Qrc,
            LyricsRawTypes.Krc => LyricsTypes.Krc,
            LyricsRawTypes.Yrc => LyricsTypes.Yrc,
            LyricsRawTypes.YrcFull => LyricsTypes.Yrc,
            LyricsRawTypes.Ttml => LyricsTypes.Ttml,
            LyricsRawTypes.AppleJson => LyricsTypes.Ttml,
            LyricsRawTypes.Spotify => LyricsTypes.Spotify,
            LyricsRawTypes.Musixmatch => LyricsTypes.Musixmatch,
            _ => LyricsTypes.Unknown,
        };
    }
}
