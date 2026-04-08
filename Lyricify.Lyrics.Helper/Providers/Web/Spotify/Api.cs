using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Lyricify.Lyrics.Providers.Web.Spotify
{
    public class Api : BaseApi
    {
        protected override string? HttpRefer => "https://open.spotify.com/";

        protected override Dictionary<string, string>? AdditionalHeaders
        {
            get
            {
                var dict = new Dictionary<string, string>
                {
                    { "Accept", "application/json" },
                    { "App-Platform", "WebPlayer" },
                    { "Origin", "https://open.spotify.com" },
                };

                if (!string.IsNullOrWhiteSpace(_accessToken))
                    dict["Authorization"] = $"Bearer {_accessToken}";

                return dict;
            }
        }

        private static readonly object _lock = new();
        private static readonly HttpClient _httpClient = new();
        private const string TokenUrl = "https://open.spotify.com/api/token";
        private const string SearchUrl = "https://api.spotify.com/v1/search";
        private const string PathfinderSearchUrl = "https://api-partner.spotify.com/pathfinder/v1/query";
        private const string ServerTimeUrl = "https://open.spotify.com/api/server-time";
        private const string SecretKeyUrl = "https://raw.githubusercontent.com/xyloflake/spot-secrets-go/main/secrets/secretDict.json";
        private const string BundledSecretJson = "{\"59\":[123,105,79,70,110,59,52,125,60,49,80,70,89,75,80,86,63,53,123,37,117,49,52,93,77,62,47,86,48,104,68,72],\"60\":[79,109,69,123,90,65,46,74,94,34,58,48,70,71,92,85,122,63,91,64,87,87],\"61\":[44,55,47,42,70,40,34,114,76,74,50,111,120,97,75,76,94,102,43,69,49,120,118,80,64,78]}";
        private const string SpotifyUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
        private static readonly string[] PathfinderSearchHashes =
        {
            "0dff51c99e552b992377a2a6f40d213dc42b62db86ca0bcf16cf3934aec1aae6",
            "75bbf6bfcfdf85b8fc828417bfad92b7cd66bf7f556d85670f4da8292373ebec",
        };

        private static string _spDc = string.Empty;
        private static string _accessToken = string.Empty;
        private static long _accessTokenExpirationTimestampMs;

        public void SetSpDc(string? spDc)
        {
            lock (_lock)
            {
                _spDc = spDc?.Trim() ?? string.Empty;
            }
        }

        public string GetSpDc()
        {
            lock (_lock)
            {
                return _spDc;
            }
        }

        public void SetAccessToken(string? token, long expirationTimestampMs = 0)
        {
            lock (_lock)
            {
                _accessToken = token?.Trim() ?? string.Empty;
                _accessTokenExpirationTimestampMs = expirationTimestampMs;
            }
        }

        public string GetAccessToken()
        {
            lock (_lock)
            {
                return _accessToken;
            }
        }

        public long GetAccessTokenExpirationTimestampMs()
        {
            lock (_lock)
            {
                return _accessTokenExpirationTimestampMs;
            }
        }

        public async Task<List<SpotifyTrackCandidate>> SearchTrackCandidates(string song, string artist, int limit = 10)
        {
            await EnsureAccessTokenAsync().ConfigureAwait(false);

            var pathfinderCandidates = await SearchTrackCandidatesViaPathfinder(song, artist, limit).ConfigureAwait(false);
            if (pathfinderCandidates.Count > 0)
            {
                return pathfinderCandidates;
            }

            return await SearchTrackCandidatesViaWebApi(song, artist, limit).ConfigureAwait(false);
        }

        public async Task<string> GetLyrics(string trackId)
        {
            var url =
                $"https://spclient.wg.spotify.com/color-lyrics/v2/track/{trackId}" +
                "?format=json&market=from_token";

            return await GetAuthorizedJsonAsync(url, request =>
            {
                request.Headers.TryAddWithoutValidation("User-Agent", SpotifyUserAgent);
                request.Headers.TryAddWithoutValidation("App-platform", "WebPlayer");
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {GetAccessToken()}");
            });
        }

        private async Task<List<SpotifyTrackCandidate>> SearchTrackCandidatesViaPathfinder(string song, string artist, int limit)
        {
            var searchTerm = string.Join(" ", new[] { song, artist }.Where(t => !string.IsNullOrWhiteSpace(t)));
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<SpotifyTrackCandidate>();
            }

            foreach (var hash in PathfinderSearchHashes)
            {
                var variablesJson = JsonConvert.SerializeObject(new
                {
                    searchTerm,
                    offset = 0,
                    limit,
                    numberOfTopResults = Math.Min(5, limit),
                });
                var extensionsJson = JsonConvert.SerializeObject(new
                {
                    persistedQuery = new
                    {
                        version = 1,
                        sha256Hash = hash,
                    },
                });

                var url =
                    PathfinderSearchUrl +
                    $"?operationName=searchDesktop&variables={WebUtility.UrlEncode(variablesJson)}&extensions={WebUtility.UrlEncode(extensionsJson)}";

                var response = await SendAsync(url, request =>
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", SpotifyUserAgent);
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {GetAccessToken()}");
                }).ConfigureAwait(false);

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    await RefreshAccessTokenAsync().ConfigureAwait(false);

                    response = await SendAsync(url, request =>
                    {
                        request.Headers.TryAddWithoutValidation("User-Agent", SpotifyUserAgent);
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {GetAccessToken()}");
                    }).ConfigureAwait(false);

                    json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    continue;
                }

                response.EnsureSuccessStatusCode();

                var parsed = ParsePathfinderTrackCandidates(json);
                if (parsed.Count > 0)
                {
                    return parsed;
                }
            }

            return new List<SpotifyTrackCandidate>();
        }

        private async Task<List<SpotifyTrackCandidate>> SearchTrackCandidatesViaWebApi(string song, string artist, int limit)
        {
            var keyword = string.Join(" ", new[] { song, artist }.Where(t => !string.IsNullOrWhiteSpace(t)));
            var url =
                SearchUrl +
                $"?q={WebUtility.UrlEncode(keyword)}&type=track&limit={limit}&market=from_token";

            var json = await GetAuthorizedJsonAsync(url, request =>
            {
                request.Headers.TryAddWithoutValidation("User-Agent", SpotifyUserAgent);
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {GetAccessToken()}");
            }).ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<SearchResponse>(json);
            return result?.Tracks?.Items?.Select(t => new SpotifyTrackCandidate
            {
                Id = t.Id ?? string.Empty,
                Title = t.Name ?? string.Empty,
                ArtistName = string.Join(", ", t.Artists?.Select(a => a.Name).Where(a => !string.IsNullOrWhiteSpace(a)) ?? Enumerable.Empty<string>()),
                AlbumName = t.Album?.Name ?? string.Empty,
                DurationMs = t.DurationMs,
            }).Where(t => !string.IsNullOrWhiteSpace(t.Id) && !string.IsNullOrWhiteSpace(t.Title)).ToList()
                ?? new List<SpotifyTrackCandidate>();
        }

        private static List<SpotifyTrackCandidate> ParsePathfinderTrackCandidates(string json)
        {
            try
            {
                var root = JObject.Parse(json);
                var arrays = new[]
                {
                    root.SelectToken("data.searchV2.tracks.items") as JArray,
                    root.SelectToken("data.search.tracks.items") as JArray,
                };

                foreach (var array in arrays.Where(a => a is { Count: > 0 }))
                {
                    var parsed = array!
                        .Select(ParsePathfinderTrackCandidate)
                        .Where(t => t is not null)
                        .Cast<SpotifyTrackCandidate>()
                        .ToList();

                    if (parsed.Count > 0)
                    {
                        return parsed;
                    }
                }
            }
            catch
            {
            }

            return new List<SpotifyTrackCandidate>();
        }

        private static SpotifyTrackCandidate? ParsePathfinderTrackCandidate(JToken raw)
        {
            var dataNode = raw["data"] ?? raw;
            var id = dataNode.Value<string>("id") ?? SpotifyIdFromUri(dataNode.Value<string>("uri"));
            var title = dataNode.Value<string>("name")
                ?? dataNode["track"]?.Value<string>("name")
                ?? string.Empty;
            var albumName = dataNode["albumOfTrack"]?.Value<string>("name")
                ?? dataNode["album"]?.Value<string>("name")
                ?? string.Empty;

            var artistItems = dataNode.SelectToken("artists.items") as JArray
                ?? dataNode.SelectToken("track.artists.items") as JArray
                ?? new JArray();
            var artistNames = artistItems
                .Select(item => item["profile"]?.Value<string>("name")
                    ?? item["data"]?.Value<string>("name")
                    ?? item["data"]?["profile"]?.Value<string>("name")
                    ?? item.Value<string>("name"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            return new SpotifyTrackCandidate
            {
                Id = id,
                Title = title,
                ArtistName = string.Join(", ", artistNames),
                AlbumName = albumName,
            };
        }

        private static string? SpotifyIdFromUri(string? uri)
        {
            const string prefix = "spotify:track:";
            if (string.IsNullOrWhiteSpace(uri) || !uri.StartsWith(prefix, StringComparison.Ordinal))
            {
                return null;
            }

            return uri[prefix.Length..];
        }

        private async Task<string> GetAuthorizedJsonAsync(string url, Action<HttpRequestMessage> configureRequest)
        {
            await EnsureAccessTokenAsync().ConfigureAwait(false);

            var response = await SendAsync(url, configureRequest).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                await RefreshAccessTokenAsync().ConfigureAwait(false);

                response = await SendAsync(url, configureRequest).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnauthorizedAccessException("Spotify access token is invalid or expired.");
                }
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private static async Task<HttpResponseMessage> SendAsync(string url, Action<HttpRequestMessage> configureRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            configureRequest(request);
            return await _httpClient.SendAsync(request).ConfigureAwait(false);
        }

        private async Task EnsureAccessTokenAsync()
        {
            string spDc;
            string accessToken;
            long expirationTimestampMs;

            lock (_lock)
            {
                spDc = _spDc;
                accessToken = _accessToken;
                expirationTimestampMs = _accessTokenExpirationTimestampMs;
            }

            if (string.IsNullOrWhiteSpace(spDc))
            {
                throw new UnauthorizedAccessException("Spotify sp_dc is not configured.");
            }

            if (!string.IsNullOrWhiteSpace(accessToken)
                && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < expirationTimestampMs)
            {
                return;
            }

            await RefreshAccessTokenAsync().ConfigureAwait(false);
        }

        private async Task RefreshAccessTokenAsync()
        {
            string spDc;
            lock (_lock)
            {
                spDc = _spDc;
            }

            if (string.IsNullOrWhiteSpace(spDc))
            {
                throw new UnauthorizedAccessException("Spotify sp_dc is not configured.");
            }

            var parameters = await BuildTokenParametersAsync().ConfigureAwait(false);
            var query = string.Join("&", parameters.Select(t => $"{WebUtility.UrlEncode(t.Key)}={WebUtility.UrlEncode(t.Value)}"));
            using var request = new HttpRequestMessage(HttpMethod.Get, TokenUrl + "?" + query);
            request.Headers.TryAddWithoutValidation("User-Agent", SpotifyUserAgent);
            request.Headers.TryAddWithoutValidation("Cookie", $"sp_dc={spDc}");

            using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException("Spotify sp_dc is invalid.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to refresh Spotify access token. HTTP {(int)response.StatusCode}: {PreviewText(text)}");
            }

            var payload = JsonConvert.DeserializeObject<SpotifyTokenResponse>(text);
            if (payload?.IsAnonymous == true || string.IsNullOrWhiteSpace(payload?.AccessToken))
            {
                throw new UnauthorizedAccessException("Spotify sp_dc is invalid.");
            }

            lock (_lock)
            {
                _accessToken = payload.AccessToken!;
                _accessTokenExpirationTimestampMs = payload.AccessTokenExpirationTimestampMs;
            }
        }

        private async Task<List<KeyValuePair<string, string>>> BuildTokenParametersAsync()
        {
            using var serverTimeRequest = new HttpRequestMessage(HttpMethod.Get, ServerTimeUrl);
            using var serverTimeResponse = await _httpClient.SendAsync(serverTimeRequest).ConfigureAwait(false);
            serverTimeResponse.EnsureSuccessStatusCode();
            var serverTimeText = await serverTimeResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var serverTime = JObject.Parse(serverTimeText)["serverTime"]?.Value<long>()
                ?? throw new InvalidOperationException("Spotify server time is invalid.");

            var (secret, version) = await FetchLatestSecretAsync().ConfigureAwait(false);
            var totp = GenerateTotp(serverTime, secret);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            return new List<KeyValuePair<string, string>>
            {
                new("reason", "transport"),
                new("productType", "web-player"),
                new("totp", totp),
                new("totpVer", version),
                new("ts", timestamp),
            };
        }

        private async Task<(string Secret, string Version)> FetchLatestSecretAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, SecretKeyUrl);
                using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (TryParseSecretPayload(raw, out var parsed))
                {
                    return parsed;
                }
            }
            catch
            {
            }

            if (TryParseSecretPayload(BundledSecretJson, out var fallback))
            {
                return fallback;
            }

            throw new InvalidOperationException("Spotify secret payload is invalid.");
        }

        private static bool TryParseSecretPayload(string raw, out (string Secret, string Version) result)
        {
            result = default;

            try
            {
                var json = JObject.Parse(raw);
                var lastProperty = json.Properties().LastOrDefault();
                if (lastProperty?.Value is not JArray array)
                {
                    return false;
                }

                var transformed = array
                    .Select((value, index) => (value?.Value<int>() ?? 0) ^ ((index % 33) + 9))
                    .Select(value => value.ToString())
                    .ToArray();

                result = (string.Concat(transformed), lastProperty.Name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GenerateTotp(long serverTimeSeconds, string secret)
        {
            const long period = 30;
            const int digits = 6;

            var counter = serverTimeSeconds / period;
            var counterBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(counter));
            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(counterBytes);
            var offset = hash[^1] & 0x0f;
            var binary = ((hash[offset] & 0x7f) << 24)
                | (hash[offset + 1] << 16)
                | (hash[offset + 2] << 8)
                | hash[offset + 3];
            var code = binary % (int)Math.Pow(10, digits);
            return code.ToString($"D{digits}");
        }

        private static string? PreviewText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var trimmed = text.Trim();
            return trimmed.Length <= 180 ? trimmed : trimmed[..180];
        }

        private sealed class SpotifyTokenResponse
        {
            [JsonProperty("accessToken")]
            public string? AccessToken { get; set; }

            [JsonProperty("accessTokenExpirationTimestampMs")]
            public long AccessTokenExpirationTimestampMs { get; set; }

            [JsonProperty("isAnonymous")]
            public bool? IsAnonymous { get; set; }
        }
    }
}
