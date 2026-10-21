using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Helpers.Optimization
{
    internal static class SyllableWordMerger
    {
        public static void Merge(SyllableLineInfo line)
        {
            if (line.Syllables.Count < 2)
            {
                return;
            }

            var merged = new List<ISyllableInfo>(line.Syllables.Count);
            foreach (var current in line.Syllables)
            {
                if (merged.Count > 0 && !HasSharedPronunciation(merged[^1]) && !HasSharedPronunciation(current)
                    && ShouldMerge(merged[^1].Text, current.Text))
                {
                    merged[^1] = MergeItems(merged[^1], current);
                }
                else
                {
                    merged.Add(current);
                }
            }

            line.Syllables = merged;
            line.RefreshProperties();
        }

        private static bool HasSharedPronunciation(ISyllableInfo item)
            => item is FullSyllableInfo group && group.GroupPronunciations.Count > 0;

        public static bool IsChineseOrJapaneseCharacter(char character)
        {
            return character is >= '\u4E00' and <= '\u9FFF'
                or >= '\u3400' and <= '\u4DBF'
                or >= '\u3040' and <= '\u309F'
                or >= '\u30A0' and <= '\u30FF'
                or >= '\u31F0' and <= '\u31FF';
        }

        private static bool ShouldMerge(string? previousText, string? currentText)
        {
            if (string.IsNullOrEmpty(previousText) || string.IsNullOrEmpty(currentText))
            {
                return false;
            }

            if (char.IsWhiteSpace(previousText[^1]) || char.IsWhiteSpace(currentText[0]))
            {
                return false;
            }

            if (previousText.Any(IsChineseOrJapaneseCharacter)
                || currentText.Any(IsChineseOrJapaneseCharacter))
            {
                return false;
            }

            return previousText.Any(char.IsLetterOrDigit)
                && currentText.Any(char.IsLetterOrDigit);
        }

        private static ISyllableInfo MergeItems(ISyllableInfo previous, ISyllableInfo current)
        {
            var currentItems = Flatten(current);
            if (previous is FullSyllableInfo fullPrevious)
            {
                fullPrevious.SubItems.AddRange(currentItems);
                fullPrevious.RefreshProperties();
                return fullPrevious;
            }

            var items = Flatten(previous);
            items.AddRange(currentItems);
            return new FullSyllableInfo(items);
        }

        private static List<SyllableInfo> Flatten(ISyllableInfo item)
        {
            if (item is FullSyllableInfo full)
            {
                return full.SubItems
                    .Select(part => PronunciationHelper.Clone(part))
                    .ToList();
            }

            var copy = new SyllableInfo(item.Text, item.StartTime, item.EndTime);
            PronunciationHelper.CopyPronunciations(item, copy);
            return new List<SyllableInfo> { copy };
        }
    }
}
