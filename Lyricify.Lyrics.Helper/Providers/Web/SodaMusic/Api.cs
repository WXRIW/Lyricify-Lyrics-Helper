﻿using Newtonsoft.Json;

namespace Lyricify.Lyrics.Providers.Web.SodaMusic
{
    public class Api : BaseApi
    {
        protected override string HttpRefer => "https://api.qishui.com/";

        protected override Dictionary<string, string>? AdditionalHeaders => null;

        public new const string UserAgent = "LunaPC/2.6.5(197449790)";

        public async Task<SearchResult?> Search(string keyword)
        {
            // search/{类型} 已知有all, track
            string url = $"https://api.qishui.com/luna/pc/search/track?aid=386088&app_name=&region=&geo_region=&os_region=&sim_region=&device_id=&cdid=&iid=&version_name=&version_code=&channel=&build_mode=&network_carrier=&ac=&tz_name=&resolution=&device_platform=&device_type=&os_version=&fp=&q={Uri.EscapeDataString(keyword)}&cursor=&search_id=&search_method=input&debug_params=&from_search_id=&search_scene=";

            var res = await GetAsync(url);

            return JsonConvert.DeserializeObject<SearchResult>(res);
        }

        public async Task<TrackDetailResult?> GetDetail(string id)
        {
            string url = $"https://api.qishui.com/luna/pc/track_v2";

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

        // 待后期歌词获取模块整理开源后并入
        //public async Task<LyricResult?> GetLyric(string id)
        //{
        //    var result = new LyricResult
        //    {
        //        Lyric = new LyricsData(),
        //        Translate = new LyricsData(),
        //    };

        //    var detail = await GetDetail(id);
        //    // Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(detail, Newtonsoft.Json.Formatting.Indented));

        //    var lyricDetail = detail?.Lyric;
        //    if (lyricDetail != null)
        //    {
        //        Enum.TryParse(lyricDetail.Type, true, out LyricsRawTypes lyricType);
        //        result.Lyric = ParseHelper.ParseLyrics(lyricDetail.Content, lyricType);
        //        // Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Lyric, Newtonsoft.Json.Formatting.Indented));
        //    }

        //    if (lyricDetail.LangTranslations.ContainsKey("ZH-HANS-CN"))
        //    {
        //        var lyricTrans = lyricDetail.LangTranslations["ZH-HANS-CN"];
        //        Enum.TryParse(lyricTrans.Type, true, out LyricsRawTypes lyricTransType);
        //        result.Translate = ParseHelper.ParseLyrics(lyricTrans.Content, lyricTransType);
        //        // Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Translate, Newtonsoft.Json.Formatting.Indented));
        //    }

        //    return result;
        //}
    }
}
