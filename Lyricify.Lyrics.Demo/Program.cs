using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;
using Lyricify.Lyrics.Searchers;

namespace Lyricify.Lyrics.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ParsersDemo();
            // GeneratorsDemo();
            // TypeDetectorDemo();
            SearchDemo();
        }

        static void ParsersDemo()
        {
            /* Parsers Demo */

            LyricsData? lyricsData;

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifySyllableDemo.txt"), LyricsRawTypes.LyricifySyllable);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LsMixQrcDemo.txt"), LyricsRawTypes.LyricifySyllable);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifyLinesDemo.txt"), LyricsRawTypes.LyricifyLines);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LrcDemo.txt"), LyricsRawTypes.Lrc);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/QrcDemo.txt"), LyricsRawTypes.Qrc);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/KrcDemo.txt"), LyricsRawTypes.Krc);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/YrcDemo.txt"), LyricsRawTypes.Yrc);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));
            Helpers.Optimization.Yrc.StandardizeYrcLyrics(lyricsData!.Lines!); // 优化 YRC 歌词
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyDemo.txt"), LyricsRawTypes.Spotify);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifySyllableDemo.txt"), LyricsRawTypes.Spotify);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyUnsyncedDemo.txt"), LyricsRawTypes.Spotify);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));
            
            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/MusixmatchDemo.txt"), LyricsRawTypes.Musixmatch);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));
            Helpers.Optimization.Musixmatch.StandardizeMusixmatchLyrics(lyricsData!.Lines!); // 优化 Musixmatch 歌词
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));
        }

        static void GeneratorsDemo()
        {
            /* Generators Demo */

            // 读取歌词数据供后期生成使用
            LyricsData? lyricsData;
            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifySyllableDemo.txt"), LyricsRawTypes.LyricifySyllable);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            string? lyrics;

            lyrics = GenerateHelper.GenerateString(lyricsData!, LyricsTypes.LyricifySyllable);
            Console.WriteLine(lyrics);

            lyrics = GenerateHelper.GenerateString(lyricsData!, LyricsTypes.LyricifyLines);
            Console.WriteLine(lyrics);

            lyrics = GenerateHelper.GenerateString(lyricsData!, LyricsTypes.Lrc);
            Console.WriteLine(lyrics);

            lyrics = GenerateHelper.GenerateString(lyricsData!, LyricsTypes.Qrc);
            Console.WriteLine(lyrics);

            lyrics = GenerateHelper.GenerateString(lyricsData!, LyricsTypes.Krc);
            Console.WriteLine(lyrics);

            lyrics = GenerateHelper.GenerateString(lyricsData!, LyricsTypes.Yrc);
            Console.WriteLine(lyrics);
        }

        static void TypeDetectorDemo()
        {
            /* Type Detector Demo */

            Console.WriteLine(Helpers.Types.Lrc.IsLrc(File.ReadAllText("RawLyrics/LrcDemo.txt")));
            Console.WriteLine(Helpers.Types.Lrc.IsLrc(File.ReadAllText("RawLyrics/QrcDemo.txt")));
        }

        static void SearchDemo()
        {
            /* Search Demo */

            var search = SearchHelper.Search(new TrackMultiArtistMetadata()
            {
                Album = "RUNAWAY",
                AlbumArtists = new() { "OneRepublic" },
                Artists = new() { "OneRepublic" },
                DurationMs = 143264,
                Title = "RUNAWAY",
            }, Searchers.Searchers.SodaMusic, Searchers.Helpers.CompareHelper.MatchType.Medium).Result;
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(search, Newtonsoft.Json.Formatting.Indented));

            var api = new Providers.Web.SodaMusic.Api();
            var lyrics = api.GetLyric((search as SodaMusicSearchResult).Id).Result;
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyrics, Newtonsoft.Json.Formatting.Indented));

            //var qqSearch = new Searchers.QQMusicSearcher();
            //var result = qqSearch.SearchForResult(new TrackMultiArtistMetadata()
            //{
            //    Album = "GUTS",
            //    AlbumArtists = new() { "Olivia Rodrigo" },
            //    Artists = new() { "Olivia Rodrigo" },
            //    DurationMs = 211141,
            //    Title = "get him back!",
            //}).Result;
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
            //var _result = qqSearch.SearchForResults(new TrackMultiArtistMetadata()
            //{
            //    Album = "RUNAWAY",
            //    AlbumArtists = new() { "OneRepublic" },
            //    Artists = new() { "OneRepublic" },
            //    DurationMs = 143264,
            //    Title = "RUNAWAY",
            //}).Result;
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(_result, Newtonsoft.Json.Formatting.Indented));

            //var neteaseSearch = new Searchers.NeteaseSearcher();
            //result = neteaseSearch.SearchForResult(new TrackMultiArtistMetadata()
            //{
            //    Album = "GUTS",
            //    AlbumArtists = new() { "Olivia Rodrigo" },
            //    Artists = new() { "Olivia Rodrigo" },
            //    DurationMs = 211141,
            //    Title = "get him back!",
            //}).Result;
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
            //_result = neteaseSearch.SearchForResults(new TrackMultiArtistMetadata()
            //{
            //    Album = "RUNAWAY",
            //    AlbumArtists = new() { "OneRepublic" },
            //    Artists = new() { "OneRepublic" },
            //    DurationMs = 143264,
            //    Title = "RUNAWAY",
            //}).Result;
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(_result, Newtonsoft.Json.Formatting.Indented));
        }
    }
}