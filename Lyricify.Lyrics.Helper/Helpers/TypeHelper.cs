using Lyricify.Lyrics.Models;
using System;

namespace Lyricify.Lyrics.Helpers
{
    public static class TypeHelper
    {
        /// <summary>
        /// 识别歌词的类型
        /// </summary>
        /// <param name="lyrics">歌词字符串</param>
        /// <returns><see cref="LyricsRawTypes"/>, 如果没有识别成功则会返回 <see cref="LyricsRawTypes.Unknown"/>.</returns>
        public static LyricsRawTypes GetLyricsTypes(string lyrics)
        {
            return Types.LyricsTypeDetector.Detect(lyrics);
        }

        /// <summary>
        /// 将 LyricsRawType 转换为 LyricsType
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

        public static bool TryParseRawType(string? name, out LyricsRawTypes type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                type = LyricsRawTypes.Unknown;
                return false;
            }

            var value = name.Trim();
            if (!char.IsDigit(value[0])
                && Enum.TryParse(value, true, out type)
                && type != LyricsRawTypes.Unknown)
            {
                return true;
            }

            type = value.ToUpperInvariant() switch
            {
                "QRC (FULL)" or "QRC (XML)" => LyricsRawTypes.QrcFull,
                "YRC (FULL)" or "YRC (JSON)" => LyricsRawTypes.YrcFull,
                "APPLE MUSIC (JSON)" or "APPLE MUSIC JSON" or "APPLE MUSIC" => LyricsRawTypes.AppleJson,
                "LYRICIFY LINE" or "LYRICIFY LINES" => LyricsRawTypes.LyricifyLines,
                "LYRICIFY SYLLABLE" or "LYRICIFY SYLLABLES" => LyricsRawTypes.LyricifySyllable,
                "MUSIXMATCH (JSON)" or "MUSIXMATCH JSON" or "MUSIXMATCHJSON" => LyricsRawTypes.Musixmatch,
                "SPOTIFY (JSON)" or "SPOTIFY JSON" or "SPOTIFYJSON" => LyricsRawTypes.Spotify,
                _ => LyricsRawTypes.Unknown,
            };
            return type != LyricsRawTypes.Unknown;
        }

        public static string GetDisplayName(this LyricsRawTypes type) => type switch
        {
            LyricsRawTypes.Lrc => "LRC",
            LyricsRawTypes.Qrc => "QRC",
            LyricsRawTypes.QrcFull => "QRC (Full)",
            LyricsRawTypes.Krc => "KRC",
            LyricsRawTypes.Yrc => "YRC",
            LyricsRawTypes.YrcFull => "YRC (Full)",
            LyricsRawTypes.Ttml => "TTML",
            LyricsRawTypes.AppleJson => "Apple Music (JSON)",
            LyricsRawTypes.LyricifyLines => "Lyricify Lines",
            LyricsRawTypes.LyricifySyllable => "Lyricify Syllable",
            LyricsRawTypes.Musixmatch => "Musixmatch (JSON)",
            LyricsRawTypes.Spotify => "Spotify (JSON)",
            _ => string.Empty,
        };

        public static string GetRawTypeDisplayName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            return TryParseRawType(name, out var type)
                ? type.GetDisplayName()
                : name.Trim();
        }

        /// <summary>
        /// 字符串是否是指定的歌词类型
        /// </summary>
        /// <param name="lyrics">歌词字符串</param>
        /// <param name="type">歌词类型</param>
        public static bool IsLyricsType(string lyrics, LyricsTypes type)
        {
            if (type == LyricsTypes.Unknown) return false;

            var rawType = GetLyricsTypes(lyrics);
            return rawType != LyricsRawTypes.Unknown && rawType.GetLyricsType() == type;
        }

        /// <summary>
        /// 字符串的歌词类型是否在指定类型列表中
        /// </summary>
        /// <param name="lyrics">歌词字符串</param>
        /// <param name="types">歌词类型列表</param>
        public static bool IsLyricsType(string lyrics, LyricsTypes[] types)
        {
            if (types is not { Length: > 0 }) return false;

            var rawType = GetLyricsTypes(lyrics);
            if (rawType == LyricsRawTypes.Unknown) return false;

            var type = rawType.GetLyricsType();
            return type != LyricsTypes.Unknown && types.Contains(type);
        }
    }
}
