using Newtonsoft.Json;

namespace Lyricify.Lyrics.Providers.Web.LRCLIB
{
    public class Api : BaseApi
    {
        protected override string? HttpRefer => null;

        protected override Dictionary<string, string>? AdditionalHeaders =>
            new()
            {
                { "User-Agent", "Lyricify-Lyrics-Helper (https://github.com/WXRIW/Lyricify-Lyrics-Helper)" }
            };

        private const string BaseUrl = "https://lrclib.net/api";

        /// <summary>
        /// 搜索歌曲
        /// </summary>
        /// <param name="trackName">曲目名</param>
        /// <param name="artistName">艺人名</param>
        /// <param name="albumName">专辑名（可选）</param>
        /// <param name="duration">时长（秒，可选）</param>
        /// <returns></returns>
        public async Task<List<SearchResultItem>?> Search(string trackName, string? artistName = null, string? albumName = null, double? duration = null)
        {
            var url = $"{BaseUrl}/search?track_name={Uri.EscapeDataString(trackName)}";

            if (!string.IsNullOrEmpty(artistName))
                url += $"&artist_name={Uri.EscapeDataString(artistName)}";

            if (!string.IsNullOrEmpty(albumName))
                url += $"&album_name={Uri.EscapeDataString(albumName)}";

            if (duration.HasValue)
                url += $"&duration={duration.Value}";

            try
            {
                var response = await GetAsync(url);
                return JsonConvert.DeserializeObject<List<SearchResultItem>>(response);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取歌词 - 通过曲目信息
        /// </summary>
        /// <param name="trackName">曲目名</param>
        /// <param name="artistName">艺人名</param>
        /// <param name="albumName">专辑名（可选）</param>
        /// <param name="duration">时长（秒，可选）</param>
        /// <returns></returns>
        public async Task<GetLyricResult?> Get(string trackName, string artistName, string? albumName = null, double? duration = null)
        {
            var url = $"{BaseUrl}/get?track_name={Uri.EscapeDataString(trackName)}&artist_name={Uri.EscapeDataString(artistName)}";

            if (!string.IsNullOrEmpty(albumName))
                url += $"&album_name={Uri.EscapeDataString(albumName)}";

            if (duration.HasValue)
                url += $"&duration={duration.Value}";

            try
            {
                var response = await GetAsync(url);
                return JsonConvert.DeserializeObject<GetLyricResult>(response);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取歌词 - 通过 ID
        /// </summary>
        /// <param name="id">LRCLIB ID</param>
        /// <returns></returns>
        public async Task<GetLyricResult?> GetById(int id)
        {
            var url = $"{BaseUrl}/get/{id}";

            try
            {
                var response = await GetAsync(url);
                return JsonConvert.DeserializeObject<GetLyricResult>(response);
            }
            catch
            {
                return null;
            }
        }
    }
}
