using Newtonsoft.Json;
using System.Text;

namespace Lyricify.Lyrics.Providers.Web
{
    public abstract class BaseApi
    {
        public static HttpClient HttpClient = new();

        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";

        public const string Cookie = "os=pc;osver=Microsoft-Windows-10-Professional-build-16299.125-64bit;appver=2.0.3.131777;channel=netease;__remember_me=true";

        protected virtual string? HttpUserAgent => UserAgent;

        protected virtual string? HttpCookie => null;

        protected abstract string? HttpRefer { get; }

        protected abstract Dictionary<string, string>? AdditionalHeaders { get; }

        protected async Task<HttpResponseMessage> GetResponseAsync(string url)
        {
            using var request = CreateRequest(HttpMethod.Get, url);
            return await HttpClient.SendAsync(request);
        }

        protected async Task<string> GetAsync(string url)
        {
            using var response = await GetResponseAsync(url);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        protected async Task<string> PostAsync(string url, Dictionary<string, string> paramDict)
        {
            using var request = CreateRequest(HttpMethod.Post, url, new FormUrlEncodedContent(paramDict));
            using var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        protected async Task<string> PostJsonAsync(string url, object param)
        {
            using var request = CreateRequest(
                HttpMethod.Post,
                url,
                new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json"));
            using var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        protected async Task<string> PostAsync(string url, Dictionary<string, object> paramDict)
        {
            using var request = CreateRequest(
                HttpMethod.Post,
                url,
                new StringContent(paramDict.ToJson(), Encoding.UTF8, "application/json"));
            using var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        protected async Task<string> PostAsync(string url, string param)
        {
            using var request = CreateRequest(
                HttpMethod.Post,
                url,
                new StringContent(param, Encoding.UTF8, "application/json"));
            using var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, url)
            {
                Content = content
            };
            SetRequestHeaders(request);
            return request;
        }

        private void SetRequestHeaders(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(HttpUserAgent))
                request.Headers.TryAddWithoutValidation("User-Agent", HttpUserAgent);
            if (!string.IsNullOrEmpty(HttpRefer))
                request.Headers.TryAddWithoutValidation("Referer", HttpRefer);
            if (!string.IsNullOrEmpty(HttpCookie))
                request.Headers.TryAddWithoutValidation("Cookie", HttpCookie);

            var additionalHeaders = AdditionalHeaders;
            if (additionalHeaders is not null)
            {
                foreach (var pair in additionalHeaders)
                {
                    request.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
                }
            }
        }
    }

    public static class JsonUtils
    {
        public static T? ToEntity<T>(this string val) => JsonConvert.DeserializeObject<T>(val);

        public static List<T>? ToEntityList<T>(this string val) => JsonConvert.DeserializeObject<List<T>>(val);

        public static string? ToJson<T>(this T entity, Formatting formatting = Formatting.None) => JsonConvert.SerializeObject(entity, formatting);
    }
}
