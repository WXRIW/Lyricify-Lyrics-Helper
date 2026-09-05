namespace Lyricify.Lyrics.Providers.Web.Musixmatch
{
    /// <summary>
    /// Musixmatch API 请求配置。
    /// </summary>
    public sealed class ApiOptions
    {
        public ApiOptions()
        {
            UseAndroid();
        }

        public string ApiBaseUrl { get; set; } = string.Empty;

        public string AppId { get; set; } = string.Empty;

        public string? UserAgent { get; set; }

        public string? Cookie { get; set; }

        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// 为每次请求生成 Musixmatch 的 t 参数。
        /// </summary>
        public Func<string> RequestIdFactory { get; set; } = null!;

        /// <summary>
        /// 在请求发出前追加或修改请求头。
        /// </summary>
        public Action<HttpRequestMessage>? ConfigureRequest { get; set; }

        /// <summary>
        /// 可选的请求发送函数。未设置时使用 API 实例自己的 HttpClient。
        /// </summary>
        public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? SendAsync { get; set; }

        public void UseAndroid()
        {
            ApiBaseUrl = "https://apic.musixmatch.com/ws/1.1/";
            AppId = "android-player-v1.0";
            UserAgent = "Dalvik/2.1.0 (Linux; U; Android 13)";
            Cookie = "AWSELB=0; AWSELBCORS=0";
            Timeout = TimeSpan.FromSeconds(4);
            RequestIdFactory = () => Guid.NewGuid().ToString("N");
            ConfigureRequest = null;
            SendAsync = null;
        }

        public void UseDesktop()
        {
            ApiBaseUrl = "https://apic-desktop.musixmatch.com/ws/1.1/";
            AppId = "web-desktop-app-v1.0";
            UserAgent = BaseApi.UserAgent;
            Cookie = "AWSELB=0; AWSELBCORS=0";
            Timeout = TimeSpan.FromSeconds(4);
            RequestIdFactory = () =>
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ConfigureRequest = null;
            SendAsync = null;
        }

        /// <summary>
        /// Android API 的别名。
        /// </summary>
        public void UseMobile()
        {
            UseAndroid();
        }
    }
}
