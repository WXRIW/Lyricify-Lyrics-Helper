using Newtonsoft.Json;

namespace Lyricify.Lyrics.Providers.Web.SodaMusic
{
    public class Api : BaseApi
    {
        protected override string HttpRefer => "https://api.qishui.com/";

        protected override Dictionary<string, string>? AdditionalHeaders => null;

        protected override string? HttpUserAgent => UserAgent;

        protected override string? HttpCookie => null;

        public new const string UserAgent = "LunaPC/2.1.0(12292405)";

        private const string SearchUserAgent = "com.luna.music/100198030 (Linux; U; Android 15; zh_CN_#Hans; ABR-AL80; Build/V417IR;tt-ok/3.12.13.19)";

        private const string WebUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

        private static readonly HttpClient SodaHttpClient = new();

        private static readonly Random Random = new();

        private static readonly string SearchDeviceId = GenerateClientId();

        private static readonly string SearchInstallId = GenerateClientId();

        public async Task<SearchResult?> Search(string keyword)
        {
            var query = new Dictionary<string, string>
            {
                { "device_platform", "android" },
                { "os", "android" },
                { "ssmix", "a" },
                { "cdid", "46556f98-1720-4248-83da-62b74b60b46a" },
                { "channel", "xiaomi_8478_64" },
                { "aid", "386088" },
                { "app_name", "luna" },
                { "version_code", "100198030" },
                { "version_name", "19.8.0" },
                { "manifest_version_code", "100198030" },
                { "update_version_code", "100198030" },
                { "resolution", "1080*1920" },
                { "dpi", "480" },
                { "device_type", "ABR-AL80" },
                { "device_brand", "HUAWEI" },
                { "language", "zh" },
                { "os_api", "35" },
                { "os_version", "15" },
                { "ac", "wifi" },
                { "device_model", "ABR-AL80" },
                { "tz_name", "Asia/Shanghai" },
                { "tz_offset", "28800" },
                { "package", "com.luna.music" },
                { "sim_region", "cn" },
                { "iid", SearchInstallId },
                { "device_id", SearchDeviceId },
                { "_rticket", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() },
                { "q", keyword },
                { "cursor", "0" },
                { "count", "20" },
            };

            using var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl("search/track", query));
            request.Headers.TryAddWithoutValidation("Accept", "*/*");
            request.Headers.TryAddWithoutValidation("User-Agent", SearchUserAgent);

            using var response = await SodaHttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(res))
                return null;

            return JsonConvert.DeserializeObject<SearchResult>(res);
        }

        public async Task<TrackDetailResult?> GetDetail(string id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                    { "track_id", id },
                    { "device_platform", "web" },
                };

                using var request = new HttpRequestMessage(HttpMethod.Get, BuildH5Url("seo_track", query));
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("User-Agent", WebUserAgent);

                using var response = await SodaHttpClient.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var resp = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = resp.ToEntity<TrackDetailResult>();

                if (result?.SeoTrack != null)
                {
                    result.Track ??= result.SeoTrack.Track;
                    result.TrackPlayer ??= result.SeoTrack.TrackPlayer;
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        private static string BuildUrl(string path, Dictionary<string, string> query)
        {
            return $"https://api.qishui.com/luna/{path}?" + string.Join("&", query.Select(pair =>
                $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}"));
        }

        private static string BuildH5Url(string path, Dictionary<string, string> query)
        {
            return $"https://beta-luna.douyin.com/luna/h5/{path}?" + string.Join("&", query.Select(pair =>
                $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}"));
        }

        private static string GenerateClientId()
        {
            lock (Random)
            {
                return Random.Next(10_000_000, 99_999_999).ToString() + Random.Next(10_000_000, 99_999_999).ToString();
            }
        }

    }
}
