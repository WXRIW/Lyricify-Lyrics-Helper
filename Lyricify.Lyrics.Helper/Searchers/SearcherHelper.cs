namespace Lyricify.Lyrics.Searchers
{
    public static class SearcherHelper
    {
        private static QQMusicSearcher? _qqMusicSearcher;

        public static QQMusicSearcher QQMusicSearcher => _qqMusicSearcher ??= new();

        private static NeteaseSearcher? _neteaseSearcher;

        public static NeteaseSearcher NeteaseSearcher => _neteaseSearcher ??= new();
    }
}
