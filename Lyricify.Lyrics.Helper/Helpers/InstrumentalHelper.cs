using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Helpers
{
    /// <summary>
    /// 根据歌词提示识别纯音乐。
    /// </summary>
    public static class InstrumentalHelper
    {
        /// <summary>
        /// 判断解析后的歌词是否只有一行已知的纯音乐提示。
        /// </summary>
        /// <param name="lyrics">解析后的歌词数据</param>
        public static bool IsInstrumental(this LyricsData? lyrics)
        {
            if (lyrics?.Lines is not { Count: 1 })
            {
                return false;
            }

            return IsInstrumental(lyrics.Lines[0].Text);
        }

        /// <summary>
        /// 判断文本是否为已知的纯音乐提示。
        /// </summary>
        /// <param name="text">不含时间戳的歌词提示文本</param>
        public static bool IsInstrumental(string? text)
            => text is "纯音乐，请欣赏"
                or "此歌曲为没有填词的纯音乐，请您欣赏"
                or "此歌曲为DJ舞曲，请您欣赏"
                or "此歌曲为没有填词的DJ舞曲，请您欣赏";
    }
}
