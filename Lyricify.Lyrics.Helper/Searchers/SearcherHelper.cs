namespace Lyricify.Lyrics.Searchers
{
    /// <summary>
    /// 实例化 Searcher 的静态类
    /// </summary>
    public static class SearcherHelper
    {
        private static QQMusicSearcher? _qqMusicSearcher;

        public static QQMusicSearcher QQMusicSearcher => _qqMusicSearcher ??= new();

        private static NeteaseSearcher? _neteaseSearcher;

        public static NeteaseSearcher NeteaseSearcher => _neteaseSearcher ??= new();

        private static KugouSearcher? _kugouSearcher;

        public static KugouSearcher KugouSearcher => _kugouSearcher ??= new();
    }
}
