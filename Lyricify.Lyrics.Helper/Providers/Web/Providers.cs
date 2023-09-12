namespace Lyricify.Lyrics.Providers.Web
{
    internal static class Providers
    {
        private static QQMusic.Api? _qqMusicApi;

        public static QQMusic.Api QQMusicApi => _qqMusicApi ??= new();

        private static Netease.Api? _neteaseApi;

        public static Netease.Api NeteaseApi => _neteaseApi ??= new();
    }
}
