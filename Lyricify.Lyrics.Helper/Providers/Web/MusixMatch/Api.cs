using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lyricify.Lyrics.Providers.Web.Musixmatch
{
    public class Api : BaseApi
    {
        private const int RequestRetryCount = 5;
        private const int ResultRetryCount = 5;

        private readonly ApiOptions options;
        private readonly HttpClient client;
        private readonly SemaphoreSlim requestLock = new(1, 1);
        private readonly SemaphoreSlim tokenLock = new(1, 1);
        private DateTime lastRequestUtc = DateTime.MinValue;
        private string? userToken;

        protected override string? HttpRefer => null;

        protected override Dictionary<string, string>? AdditionalHeaders => null;

        /// <summary>
        /// 使用 Android API 创建实例。
        /// </summary>
        public Api() : this(null)
        {
        }

        /// <summary>
        /// 使用调用方注入的配置创建实例。
        /// </summary>
        /// <param name="configure">
        /// 配置委托。可调用 <see cref="ApiOptions.UseAndroid"/> 或
        /// <see cref="ApiOptions.UseDesktop"/>，也可进一步覆盖具体配置。
        /// </param>
        public Api(Action<ApiOptions>? configure)
        {
            options = new ApiOptions();
            configure?.Invoke(options);
            ValidateOptions(options);
            client = CreateClient(options);
        }

        public void SetUserToken(string token)
        {
            userToken = IsUsableToken(token) ? token : null;
        }

        public string? GetUserToken()
        {
            return userToken;
        }

        public async Task<GetTokenResponse?> GetToken()
        {
            var response = await RequestTokenAsync(CancellationToken.None);
            return response is null
                ? null
                : JsonConvert.DeserializeObject<GetTokenResponse>(response.ToString(Formatting.None));
        }

        public async Task<GetTrackResponse?> GetTrack(string track, string artist, int? duration = null)
        {
            var tracks = await SearchTracksAsync(
                null,
                track,
                artist,
                duration,
                CancellationToken.None);
            var result = tracks.FirstOrDefault();
            if (result is null)
            {
                return null;
            }

            return new GetTrackResponse
            {
                Message = new GetTrackResponse.MessageContent
                {
                    Header = new GetTrackResponse.Header
                    {
                        StatusCode = 200,
                        Confidence = 1000,
                    },
                    Body = new GetTrackResponse.Body
                    {
                        Track = result,
                    },
                },
            };
        }

        public async Task<IReadOnlyList<GetTrackResponse.Track>> SearchTracksAsync(
            string? keyword,
            string? track,
            string? artist,
            int? duration,
            CancellationToken cancellationToken)
        {
            var parameters = new List<string>
            {
                "page_size=10",
                "page=1",
                "s_track_rating=desc",
            };
            AddParameter(parameters, "q", keyword);
            AddParameter(parameters, "q_track", track);
            AddParameter(parameters, "q_artist", artist);
            if (duration is > 0)
            {
                parameters.Add($"q_duration={duration.Value}");
            }

            var request = "track.search?" + string.Join("&", parameters);
            for (var attempt = 0; attempt < ResultRetryCount; attempt++)
            {
                var response = await SendApiRequestAsync(request, cancellationToken);
                if (GetBody(response)?["track_list"] is JArray list)
                {
                    var results = list
                        .Select(item => item["track"]?.ToObject<GetTrackResponse.Track>())
                        .Where(item => item is not null)
                        .Select(item => item!)
                        .ToList();
                    if (results.Count > 0 && HasRelatedResult(results, keyword, track, artist))
                    {
                        return results;
                    }
                }

                if (attempt + 1 < ResultRetryCount)
                {
                    await DelayBeforeResultRetryAsync(attempt, cancellationToken);
                }
            }

            return Array.Empty<GetTrackResponse.Track>();
        }

        public async Task<GetTrackResponse.Track?> ResolveTrackAsync(
            string identifier,
            CancellationToken cancellationToken)
        {
            if (int.TryParse(identifier, out var trackId))
            {
                var response = await SendApiRequestAsync(
                    $"track.get?track_id={trackId}",
                    cancellationToken);
                var track = GetBody(response)?["track"]?.ToObject<GetTrackResponse.Track>();
                if (track?.TrackId == trackId)
                {
                    return track;
                }

                track = GetMatchedTrack(await GetLyricsResponseAsync(trackId, cancellationToken));
                return track?.TrackId == trackId ? track : null;
            }

            var vanity = NormalizeVanity(identifier);
            var parts = vanity.Split(new[] { '/' }, 2);
            if (parts.Length != 2)
            {
                return null;
            }

            var artist = DecodeVanityPart(parts[0]);
            var title = DecodeVanityPart(parts[1]);
            var results = await SearchTracksAsync(null, title, artist, null, cancellationToken);
            return results.FirstOrDefault(result =>
                    NormalizeVanity(result.CommontrackVanityId)
                        .Equals(vanity, StringComparison.OrdinalIgnoreCase))
                ?? results.FirstOrDefault(result =>
                    result.TrackName.Equals(title, StringComparison.OrdinalIgnoreCase)
                    && result.ArtistName.Split(new[] { " feat. ", " & " }, StringSplitOptions.RemoveEmptyEntries)
                        .Any(value => value.Equals(artist, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<GetTrackResponse?> GetFullLyrics(
            string track,
            string artist,
            int? duration = null)
        {
            var response = await GetFullLyricsRaw(track, artist, duration);
            return response is null ? null : JsonConvert.DeserializeObject<GetTrackResponse>(response);
        }

        public Task<string?> GetFullLyricsRaw(string trackId)
        {
            return GetFullLyricsRaw(trackId, null, CancellationToken.None);
        }

        public async Task<string?> GetFullLyricsRaw(
            string trackId,
            string? expectedVanityId,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(trackId, out var id))
            {
                return null;
            }

            for (var attempt = 0; attempt < ResultRetryCount; attempt++)
            {
                var response = await GetLyricsResponseAsync(id, cancellationToken);
                var matched = GetMatchedTrack(response);
                if (matched?.TrackId == id
                    && (string.IsNullOrWhiteSpace(expectedVanityId)
                        || NormalizeVanity(matched.CommontrackVanityId)
                            .Equals(NormalizeVanity(expectedVanityId), StringComparison.OrdinalIgnoreCase)))
                {
                    return response!.ToString(Formatting.None);
                }

                if (attempt + 1 < ResultRetryCount)
                {
                    await DelayBeforeResultRetryAsync(attempt, cancellationToken);
                }
            }

            return null;
        }

        public async Task<string?> GetFullLyricsRaw(
            string track,
            string artist,
            int? duration = null)
        {
            var tracks = await SearchTracksAsync(
                null,
                track,
                artist,
                duration,
                CancellationToken.None);
            var result = tracks.FirstOrDefault();
            return result is null
                ? null
                : await GetFullLyricsRaw(
                    result.TrackId.ToString(),
                    result.CommontrackVanityId,
                    CancellationToken.None);
        }

        public async Task<GetTranslationsResponse?> GetTranslations(
            string trackId,
            string language = "zh")
        {
            var response = await GetTranslationsRaw(trackId, language, CancellationToken.None);
            return response is null
                ? null
                : JsonConvert.DeserializeObject<GetTranslationsResponse>(response);
        }

        public async Task<string?> GetTranslationsRaw(
            string trackId,
            string language,
            CancellationToken cancellationToken)
        {
            var response = await SendApiRequestAsync(
                "crowd.track.translations.get?translation_fields_set=minimal" +
                $"&selected_language={Uri.EscapeDataString(language)}" +
                $"&track_id={Uri.EscapeDataString(trackId)}" +
                "&comment_format=text&part=user",
                cancellationToken);
            return response?.ToString(Formatting.None);
        }

        private Task<JObject?> GetLyricsResponseAsync(
            int trackId,
            CancellationToken cancellationToken)
        {
            return SendApiRequestAsync(
                "macro.subtitles.get?namespace=lyrics_richsynched" +
                "&optional_calls=track.richsync" +
                "&subtitle_format=lrc" +
                $"&track_id={trackId}" +
                "&f_subtitle_length_max_deviation=40",
                cancellationToken);
        }

        private async Task<JObject?> SendApiRequestAsync(
            string request,
            CancellationToken cancellationToken)
        {
            Exception? lastError = null;
            for (var attempt = 0; attempt < RequestRetryCount; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var token = await EnsureUserTokenAsync(cancellationToken);
                    var separator = request.Contains('?') ? '&' : '?';
                    var url = options.ApiBaseUrl + request + separator +
                        $"usertoken={Uri.EscapeDataString(token)}" +
                        "&format=json" +
                        $"&app_id={Uri.EscapeDataString(options.AppId)}" +
                        $"&t={Uri.EscapeDataString(options.RequestIdFactory())}";
                    var response = await GetResponseAsync(url, cancellationToken);
                    if (string.IsNullOrWhiteSpace(response.Content))
                    {
                        throw new HttpRequestException(
                            $"Musixmatch returned HTTP {response.StatusCode}.");
                    }

                    var json = JObject.Parse(response.Content);
                    var header = json["message"]?["header"];
                    var statusCode = header?["status_code"]?.Value<int>();
                    var hint = header?["hint"]?.Value<string>();
                    if (statusCode == 404)
                    {
                        return json;
                    }
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(
                            $"Musixmatch returned HTTP {response.StatusCode}.");
                    }
                    if (statusCode == 200)
                    {
                        return json;
                    }

                    if (statusCode == 401 &&
                        hint?.Equals("renew", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        InvalidateToken();
                    }
                    else if (statusCode == 401 &&
                        hint?.Equals("captcha", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        throw new RequestCaptchaException();
                    }
                    lastError = new HttpRequestException(
                        $"Musixmatch returned API status {statusCode?.ToString() ?? "unknown"}" +
                        (string.IsNullOrWhiteSpace(hint) ? "." : $" ({hint})."));
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (RequestCaptchaException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }

                if (attempt + 1 < RequestRetryCount)
                {
                    await DelayBeforeRequestRetryAsync(attempt, cancellationToken);
                }
            }

            throw new HttpRequestException(
                "Musixmatch request failed after all retries.",
                lastError);
        }

        private async Task<string> EnsureUserTokenAsync(CancellationToken cancellationToken)
        {
            if (IsUsableToken(userToken))
            {
                return userToken!;
            }

            await tokenLock.WaitAsync(cancellationToken);
            try
            {
                if (IsUsableToken(userToken))
                {
                    return userToken!;
                }

                var response = await RequestTokenAsync(cancellationToken);
                var token = response?["message"]?["body"]?["user_token"]?.Value<string>();
                if (IsUsableToken(token))
                {
                    userToken = token;
                    return token!;
                }

                throw new InvalidOperationException("Musixmatch token request failed.");
            }
            finally
            {
                tokenLock.Release();
            }
        }

        private async Task<JObject?> RequestTokenAsync(CancellationToken cancellationToken)
        {
            var url = options.ApiBaseUrl +
                "token.get?user_language=en" +
                $"&app_id={Uri.EscapeDataString(options.AppId)}" +
                $"&t={Uri.EscapeDataString(options.RequestIdFactory())}";
            var response = await GetResponseAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Musixmatch returned HTTP {response.StatusCode}.");
            }
            if (string.IsNullOrWhiteSpace(response.Content))
            {
                return null;
            }

            var json = JObject.Parse(response.Content);
            var header = json["message"]?["header"];
            var statusCode = header?["status_code"]?.Value<int>();
            var hint = header?["hint"]?.Value<string>();
            if (statusCode == 401 &&
                hint?.Equals("captcha", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new RequestCaptchaException();
            }
            if (statusCode != 200)
            {
                throw new HttpRequestException(
                    $"Musixmatch returned API status {statusCode?.ToString() ?? "unknown"}" +
                    (string.IsNullOrWhiteSpace(hint) ? "." : $" ({hint})."));
            }
            return json;
        }

        private async Task<(bool IsSuccessStatusCode, int StatusCode, string Content)> GetResponseAsync(
            string url,
            CancellationToken cancellationToken)
        {
            await requestLock.WaitAsync(cancellationToken);
            try
            {
                var elapsed = DateTime.UtcNow - lastRequestUtc;
                if (elapsed < TimeSpan.FromMilliseconds(250))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250) - elapsed, cancellationToken);
                }

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (!string.IsNullOrWhiteSpace(options.UserAgent))
                {
                    request.Headers.TryAddWithoutValidation(
                        "User-Agent",
                        options.UserAgent);
                }
                if (!string.IsNullOrWhiteSpace(options.Cookie))
                {
                    request.Headers.TryAddWithoutValidation(
                        "Cookie",
                        options.Cookie);
                }
                options.ConfigureRequest?.Invoke(request);
                using var response = options.SendAsync is null
                    ? await client.SendAsync(request, cancellationToken)
                    : await options.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                lastRequestUtc = DateTime.UtcNow;
                return (
                    response.IsSuccessStatusCode,
                    (int)response.StatusCode,
                    content);
            }
            finally
            {
                requestLock.Release();
            }
        }

        private static GetTrackResponse.Track? GetMatchedTrack(JObject? response)
        {
            var calls = GetBody(response)?["macro_calls"] as JObject;
            var matcher = calls?["matcher.track.get"] as JObject;
            var message = matcher?["message"] as JObject;
            var body = message?["body"] as JObject;
            return body?["track"]?.ToObject<GetTrackResponse.Track>();
        }

        private static JObject? GetBody(JObject? response)
        {
            return (response?["message"] as JObject)?["body"] as JObject;
        }

        private static bool IsUsableToken(string? token)
        {
            return !string.IsNullOrWhiteSpace(token)
                && token != "null"
                && token.Any(character => character != '0');
        }

        private void InvalidateToken()
        {
            userToken = null;
        }

        private static Task DelayBeforeRequestRetryAsync(
            int attempt,
            CancellationToken cancellationToken)
        {
            var delay = Math.Min(500 * (1 << attempt), 2000);
            return Task.Delay(delay, cancellationToken);
        }

        private static Task DelayBeforeResultRetryAsync(
            int attempt,
            CancellationToken cancellationToken)
        {
            return Task.Delay(Math.Min(200 * (attempt + 1), 800), cancellationToken);
        }

        private static HttpClient CreateClient(ApiOptions options)
        {
            var client = new HttpClient
            {
                Timeout = options.Timeout,
            };
            return client;
        }

        private static void ValidateOptions(ApiOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ApiBaseUrl))
            {
                throw new ArgumentException("Musixmatch API base URL is required.");
            }
            if (!options.ApiBaseUrl.EndsWith("/", StringComparison.Ordinal))
            {
                options.ApiBaseUrl += "/";
            }
            if (string.IsNullOrWhiteSpace(options.AppId))
            {
                throw new ArgumentException("Musixmatch app ID is required.");
            }
            if (options.Timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(options.Timeout),
                    "Musixmatch request timeout must be greater than zero.");
            }
            if (options.RequestIdFactory is null)
            {
                throw new ArgumentException("Musixmatch request ID factory is required.");
            }
        }

        private static void AddParameter(
            ICollection<string> parameters,
            string name,
            string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                parameters.Add($"{name}={Uri.EscapeDataString(value)}");
            }
        }

        private static string NormalizeVanity(string? value)
        {
            return Uri.UnescapeDataString(value ?? string.Empty).Trim().Trim('/');
        }

        private static string DecodeVanityPart(string value)
        {
            return Uri.UnescapeDataString(value).Replace('-', ' ').Trim();
        }

        private static bool HasRelatedResult(
            IReadOnlyCollection<GetTrackResponse.Track> results,
            string? keyword,
            string? title,
            string? artist)
        {
            var keywordTokens = Tokenize(keyword);
            var titleTokens = Tokenize(title);
            var artistTokens = Tokenize(artist);
            if (keywordTokens.Length == 0
                && titleTokens.Length == 0
                && artistTokens.Length == 0)
            {
                return true;
            }

            return results.Any(result =>
            {
                var actualTitle = result.TrackName.ToLowerInvariant();
                var actualArtists = result.ArtistName.ToLowerInvariant();
                if (titleTokens.Length > 0
                    && !titleTokens.All(actualTitle.Contains))
                {
                    return false;
                }
                if (artistTokens.Length > 0
                    && !artistTokens.Any(actualArtists.Contains))
                {
                    return false;
                }

                if (keywordTokens.Length == 0)
                {
                    return true;
                }
                var actual = $"{actualTitle} {actualArtists}";
                var requiredMatches = Math.Max(
                    1,
                    (int)Math.Ceiling(keywordTokens.Length * 0.6));
                return keywordTokens.Count(actual.Contains) >= requiredMatches;
            });
        }

        private static string[] Tokenize(string? value)
        {
            return (value ?? string.Empty)
                .ToLowerInvariant()
                .Split(
                    new[] { ' ', '-', '_', '/', ',', '.', '(', ')', '[', ']', '&' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(token => token.Length > 1)
                .ToArray();
        }
    }

    public class RequestCaptchaException : Exception
    {
        public const string DefaultMessage = "Hit 401 error with Captcha hint.";

        public RequestCaptchaException() : base(DefaultMessage) { }

        public RequestCaptchaException(string requestUrl, string response) : base(DefaultMessage)
        {
            RequestUrl = requestUrl;
            Response = response;
        }

        public RequestCaptchaException(Exception innerException) : base(DefaultMessage, innerException) { }

        public string? RequestUrl { get; private set; }

        public string? Response { get; private set; }
    }
}
