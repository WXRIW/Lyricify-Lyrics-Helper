namespace Lyricify.Lyrics.Models
{
    public class SyllableInfo : ISyllableInfo
    {
#pragma warning disable CS8618
        public SyllableInfo() { }
#pragma warning restore CS8618

        public SyllableInfo(string text, int startTime, int endTime)
        {
            Text = text;
            StartTime = startTime;
            EndTime = endTime;
        }

        public string Text { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        /// <summary>Language-tagged pronunciation text for this lyric unit, independent of its source format.</summary>
        public Dictionary<string, List<string>> Pronunciations { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public bool ShouldSerializePronunciations() => Pronunciations is { Count: > 0 };
    }

    public class FullSyllableInfo : ISyllableInfo
    {
#pragma warning disable CS8618
        public FullSyllableInfo() { }
#pragma warning restore CS8618

        public FullSyllableInfo(IEnumerable<SyllableInfo> syllableInfos)
        {
            SubItems = syllableInfos.ToList();
        }

        private string? _text = null;
        public string Text => _text ??= SyllableHelper.GetTextFromSyllableList(SubItems);

        private int? _startTime = null;
        public int StartTime => _startTime ??= SubItems.First().StartTime;

        private int? _endTime = null;
        public int EndTime => _endTime ??= SubItems.Last().EndTime;

        public List<SyllableInfo> SubItems { get; set; }

        /// <summary>Language-tagged pronunciations shared by all lyric units in this group, independent of source format.</summary>
        public Dictionary<string, List<string>> GroupPronunciations { get; } = new(StringComparer.OrdinalIgnoreCase);

        public bool ShouldSerializeGroupPronunciations() => GroupPronunciations.Count > 0;

        /// <summary>Read-only view using group-owned text for each language, or aggregating child pronunciations.</summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Pronunciations =>
            Helpers.PronunciationHelper.Aggregate(SubItems, GroupPronunciations);

        public bool ShouldSerializePronunciations() => Pronunciations.Count > 0;

        /// <summary>
        /// Refresh preloaded properties if SubItems have been updated
        /// </summary>
        public void RefreshProperties()
        {
            _text = null;
            _startTime = null;
            _endTime = null;
        }
    }

    public static class SyllableHelper
    {
        public static string GetTextFromSyllableList(List<ISyllableInfo> syllableList) => string.Concat(syllableList.Select(t => t.Text).ToArray());

        public static string GetTextFromSyllableList(List<SyllableInfo> syllableList) => string.Concat(syllableList.Select(t => t.Text).ToArray());

        public static string GetTextFromSyllableList(List<FullSyllableInfo> syllableList) => string.Concat(syllableList.Select(t => t.Text).ToArray());
    }
}
