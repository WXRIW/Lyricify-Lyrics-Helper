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

        private static readonly Random Random = new();

        private static readonly string DeviceId = GenerateClientId("738");

        private static readonly string InstallId = GenerateClientId("739");

        private static Dictionary<string, string> PcCommonParams => new()
        {
            { "aid", "386088" },
            { "app_name", "luna_pc" },
            { "device_id", DeviceId },
            { "install_id", InstallId },
            { "did", DeviceId },
            { "iid", InstallId },
            { "device_platform", "PC" },
            { "version_code", "2.1.0" },
            { "version_name", "2.1.0" },
        };

        public async Task<SearchResult?> Search(string keyword)
        {
            var query = new Dictionary<string, string>(PcCommonParams)
            {
                { "region", "" },
                { "geo_region", "" },
                { "os_region", "" },
                { "sim_region", "" },
                { "cdid", "" },
                { "channel", "" },
                { "build_mode", "" },
                { "network_carrier", "" },
                { "ac", "" },
                { "tz_name", "" },
                { "resolution", "" },
                { "device_type", "pc" },
                { "os_version", "" },
                { "fp", "" },
                { "q", keyword },
                { "cursor", "" },
                { "search_id", "" },
                { "search_method", "input" },
                { "debug_params", "" },
                { "from_search_id", "" },
                { "search_scene", "" },
            };

            var res = await GetAsync(BuildPcUrl("search/track", query));

            return JsonConvert.DeserializeObject<SearchResult>(res);
        }

        public async Task<TrackDetailResult?> GetDetail(string id)
        {
            string url = BuildPcUrl("track_v2", PcCommonParams);

            var data = new Dictionary<string, string>
            {
                { "track_id", id },
                { "media_type", "track" },
                { "queue_type", "" },
            };

            try
            {
                var resp = await PostAsync(url, data);

                return resp.ToEntity<TrackDetailResult>();
            }
            catch
            {
                return null;
            }
        }

        private static string BuildPcUrl(string path, Dictionary<string, string> query)
        {
            return $"https://api.qishui.com/luna/pc/{path}?" + string.Join("&", query.Select(pair =>
                $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}"));
        }

        private static string GenerateClientId(string prefix)
        {
            lock (Random)
            {
                return prefix + Random.Next(10_000_000, 99_999_999).ToString() + Random.Next(10_000_000, 99_999_999).ToString();
            }
        }
    }
}