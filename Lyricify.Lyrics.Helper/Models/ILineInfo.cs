namespace Lyricify.Lyrics.Models
{
    public interface ILineInfo : IComparable
    {
        public string Text { get; }

        public int? StartTime { get; }

        public int? EndTime { get; }

        public int? Duration { get; }

        public int? StartTimeWithSubLine { get; }

        public int? EndTimeWithSubLine { get; }

        public int? DurationWithSubLine { get; }

        public LyricsAlignment LyricsAlignment { get; }

        public ILineInfo? SubLine { get; }

        /// <summary>
        /// <see cref="Text"/> if SubLine not exist, or full lyrics with bracketed subline lyrics
        /// </summary>
        public string FullText { get; }
    }

    public interface IFullLineInfo : ILineInfo
    {
        public Dictionary<string, string> Translations { get; }

        public string? ChineseTranslation { get; }

        public string? Pronunciation { get; }
    }
}
