using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Helpers.Optimization
{
    public static class Musixmatch
    {
        /// <summary>
        /// 针对 Musixmatch richsync 歌词格式的优化。
        /// 将同一单词内的连续音节组合为 FullSyllableInfo，并把空白附加到前一个单词。
        /// </summary>
        public static void StandardizeMusixmatchLyrics(List<ILineInfo> list)
        {
            foreach (ILineInfo line in list)
            {
                if (line is SyllableLineInfo syllableLine)
                {
                    StandardizeMusixmatchLyrics(syllableLine);
                }
            }
        }

        /// <summary>
        /// 针对 Musixmatch richsync 歌词格式的优化。
        /// </summary>
        public static void StandardizeMusixmatchLyrics(SyllableLineInfo syllableLine)
        {
            if (syllableLine.Syllables.Count == 0
                || syllableLine.Syllables.Any(item => item is not SyllableInfo))
            {
                return;
            }

            // Musixmatch exposes whitespace as its own timed fragment. Attach it to
            // the previous fragment so the shared merger can use it as a word boundary.
            for (var index = 1; index < syllableLine.Syllables.Count; index++)
            {
                if (syllableLine.Syllables[index].Text.All(char.IsWhiteSpace))
                {
                    ((SyllableInfo)syllableLine.Syllables[index - 1]).Text +=
                        syllableLine.Syllables[index].Text;
                    syllableLine.Syllables.RemoveAt(index--);
                }
            }

            SyllableWordMerger.Merge(syllableLine);
        }
    }
}
