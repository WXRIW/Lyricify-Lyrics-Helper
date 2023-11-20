using Newtonsoft.Json;

namespace Lyricify.Lyrics.Providers.Web.Musixmatch
{
    public class Api : BaseApi
    {
        protected override string? HttpRefer => null;

        private string? _userToken { get; set; } = null;

        /// <summary>
        /// 设置 UserToken (例如之前缓存的 Token)
        /// </summary>
        public void SetUserToken(string token)
        {
            _userToken = token;
        }

        /// <summary>
        /// 获取当前的 UserToken (例如用于缓存)
        /// </summary>
        public string? GetUserToken()
        {
            return _userToken;
        }

        public async Task<GetTokenResponse?> GetToken()
        {
            var response = await HttpClient.GetStringAsync("https://apic-desktop.musixmatch.com/ws/1.1/token.get?app_id=web-desktop-app-v1.0");
            var resp = JsonConvert.DeserializeObject<GetTokenResponse>(response);
            return resp;
        }

        /// <summary>
        /// 获取曲目
        /// </summary>
        /// <param name="track">曲目</param>
        /// <param name="artist">艺人</param>
        /// <param name="duration">曲目市场 (秒)</param>
        public async Task<GetTrackResponse?> GetTrack(string track, string artist, int? duration = null)
        {
            await EnsureUserToken();

            var response = await HttpClient.GetStringAsync("https://apic-desktop.musixmatch.com/ws/1.1/matcher.track.get?format=json" +
                $"&q_track={track}" +
                $"&q_artist={artist}" +
                (duration.HasValue ? $"&f_subtitle_length={duration}&q_duration={duration}" : string.Empty) +
                $"&usertoken={_userToken}" +
                "&f_subtitle_length_max_deviation=40&app_id=web-desktop-app-v1.0");
            var resp = JsonConvert.DeserializeObject<GetTrackResponse>(response);

            if (resp?.Message.Header.StatusCode == 401 && resp?.Message.Header.Hint == "renew")
            {
                _userToken = null;
                return await GetTrack(track, artist, duration);
            }
            return resp;
        }

        /// <summary>
        /// 获取完整歌词
        /// </summary>
        /// <param name="track">曲目</param>
        /// <param name="artist">艺人</param>
        /// <param name="duration">曲目市场 (秒)</param>
        public async Task<GetTrackResponse?> GetFullLyrics(string track, string artist, int? duration = null)
        {
            await EnsureUserToken();

            var response = await GetFullLyricsRaw(track, artist, duration);
            if (response == null) return null;
            var resp = JsonConvert.DeserializeObject<GetTrackResponse>(response);
            return resp;
        }

        /// <summary>
        /// 获取完整歌词 (返回的原始字符串)
        /// </summary>
        /// <param name="trackId">曲目 ID</param>
        /// <returns>接口返回的原始字符串，可直接用 MusixmatchParser 解析</returns>
        public async Task<string?> GetFullLyricsRaw(string trackId)
        {
            await EnsureUserToken();

            var response = await HttpClient.GetStringAsync("https://apic-desktop.musixmatch.com/ws/1.1/macro.subtitles.get?format=json&namespace=lyrics_richsynched&optional_calls=track.richsync&subtitle_format=lrc" +
                $"&track_id={trackId}" +
                $"&usertoken={_userToken}" +
                "&f_subtitle_length_max_deviation=40&app_id=web-desktop-app-v1.0");

            if (response?.Contains("\"status_code\":401") == true
                && response?.Contains("\"hint\":\"renew\"") == true)
            {
                _userToken = null;
                return await GetFullLyricsRaw(trackId);
            }
            return response;
        }

        /// <summary>
        /// 获取完整歌词 (返回的原始字符串)
        /// </summary>
        /// <param name="track">曲目</param>
        /// <param name="artist">艺人</param>
        /// <param name="duration">曲目市场 (秒)</param>
        /// <returns>接口返回的原始字符串，可直接用 MusixmatchParser 解析</returns>
        public async Task<string?> GetFullLyricsRaw(string track, string artist, int? duration = null)
        {
            await EnsureUserToken();

            var response = await HttpClient.GetStringAsync("https://apic-desktop.musixmatch.com/ws/1.1/macro.subtitles.get?format=json&namespace=lyrics_richsynched&optional_calls=track.richsync&subtitle_format=lrc" +
                $"&q_track={track}" +
                $"&q_artist={artist}" +
                (duration.HasValue ? $"&f_subtitle_length={duration}&q_duration={duration}" : string.Empty) +
                $"&usertoken={_userToken}" +
                "&f_subtitle_length_max_deviation=40&app_id=web-desktop-app-v1.0");

            if (response?.Contains("\"status_code\":401") == true
                && response?.Contains("\"hint\":\"renew\"") == true)
            {
                _userToken = null;
                return await GetFullLyricsRaw(track, artist, duration);
            }
            return response;
        }

        /// <summary>
        /// 获取翻译
        /// </summary>
        /// <param name="trackId">曲目 ID</param>
        /// <param name="language">翻译的语言</param>
        public async Task<GetTranslationsResponse?> GetTranslations(string trackId, string language = "zh")
        {
            await EnsureUserToken();

            var response = await HttpClient.GetStringAsync("https://apic-desktop.musixmatch.com/ws/1.1/crowd.track.translations.get?translation_fields_set=minimal" +
                $"&selected_language={language}" +
                $"&track_id={trackId}" +
                $"&usertoken={_userToken}" +
                "&comment_format=text&part=user&format=json&app_id=web-desktop-app-v1.0");
            var resp = JsonConvert.DeserializeObject<GetTranslationsResponse>(response);

            if (resp?.Message.Header.StatusCode == 401 && resp?.Message.Header.Hint == "renew")
            {
                _userToken = null;
                return await GetTranslations(trackId, language);
            }
            return resp;
        }

        /// <summary>
        /// 确保有 UserToken
        /// </summary>
        public async Task EnsureUserToken()
        {
            if (_userToken != null) return;
            await RefreshUserToken(true);
        }

        /// <summary>
        /// 刷新 UserToken，可用于失效时的刷新
        /// </summary>
        /// <returns></returns>
        public async Task RefreshUserToken(bool isEnsure = false)
        {
            var response = await GetToken();
            int maxTry = 10;
            while (response?.Message.Header.StatusCode == 401 && response?.Message.Header.Hint == "captcha" && maxTry-- > 0)
            {
                await Task.Delay(1000);
                if (isEnsure && !string.IsNullOrEmpty(_userToken)) return;
                response = await GetToken();
            }
            if (string.IsNullOrEmpty(response?.Message?.Body?.UserToken))
                throw new Exception("User Token failed to refresh");

            _userToken = response?.Message?.Body?.UserToken;
        }
    }
}
