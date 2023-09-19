using System.Net;

namespace Lyricify.Lyrics.Providers.Web
{
    public static class Proxy
    {
        public static void SetProxy(string host, int port, string? username, string? password)
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy(host, port)
            };
            if (!string.IsNullOrEmpty(username))
                handler.Proxy.Credentials = new NetworkCredential(username, password);

            BaseApi.HttpClient = new(handler);
        }

        public static void ClearProxy()
        {
            BaseApi.HttpClient = new();
        }
    }
}
