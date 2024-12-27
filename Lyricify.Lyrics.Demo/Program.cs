using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParsersDemo();
            GeneratorsDemo();
            TypeDetectorDemo();
            SearchDemo();
        }

        static void ParsersDemo()
        {
            /* Parsers Demo */

            LyricsData? lyricsData;

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifySyllableDemo.txt"), LyricsRawTypes.LyricifySyllable);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LsMixQrcDemo.txt"), LyricsRawTypes.LyricifySyllable);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifyLinesDemo.txt"), LyricsRawTypes.LyricifyLines);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LrcDemo.txt"), LyricsRawTypes.Lrc);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/QrcDemo.txt"), LyricsRawTypes.Qrc);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/KrcDemo.txt"), LyricsRawTypes.Krc);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/YrcDemo.txt"), LyricsRawTypes.Yrc);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));
            Helpers.Optimization.Yrc.StandardizeYrcLyrics(lyricsData!.Lines!); // 优化 YRC 歌词
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyDemo.txt"), LyricsRawTypes.Spotify);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifySyllableDemo.txt"), LyricsRawTypes.Spotify);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyUnsyncedDemo.txt"), LyricsRawTypes.Spotify);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/MusixmatchDemo.txt"), LyricsRawTypes.Musixmatch);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));
            Helpers.Optimization.Musixmatch.StandardizeMusixmatchLyrics(lyricsData!.Lines!); // 优化 Musixmatch 歌词
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));
        }

        static void GeneratorsDemo()
        {
            /* Generators Demo */

            // 读取歌词数据供后期生成使用
            LyricsData? lyricsData;
            lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifySyllableDemo.txt"), LyricsRawTypes.LyricifySyllable);
            Console.WriteLine(JsonConvert.SerializeObject(lyricsData));

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
            }, Searchers.Searchers.Netease, Searchers.Helpers.CompareHelper.MatchType.Medium).Result;
            Console.WriteLine(JsonConvert.SerializeObject(search));

            //var qqSearch = new Searchers.QQMusicSearcher();
            //var result = qqSearch.SearchForResult(new TrackMultiArtistMetadata()
            //{
            //    Album = "GUTS",
            //    AlbumArtists = new() { "Olivia Rodrigo" },
            //    Artists = new() { "Olivia Rodrigo" },
            //    DurationMs = 211141,
            //    Title = "get him back!",
            //}).Result;
            //Console.WriteLine(JsonConvert.SerializeObject(result));
            //var _result = qqSearch.SearchForResults(new TrackMultiArtistMetadata()
            //{
            //    Album = "RUNAWAY",
            //    AlbumArtists = new() { "OneRepublic" },
            //    Artists = new() { "OneRepublic" },
            //    DurationMs = 143264,
            //    Title = "RUNAWAY",
            //}).Result;
            //Console.WriteLine(JsonConvert.SerializeObject(_result));

            //var neteaseSearch = new Searchers.NeteaseSearcher();
            //result = neteaseSearch.SearchForResult(new TrackMultiArtistMetadata()
            //{
            //    Album = "GUTS",
            //    AlbumArtists = new() { "Olivia Rodrigo" },
            //    Artists = new() { "Olivia Rodrigo" },
            //    DurationMs = 211141,
            //    Title = "get him back!",
            //}).Result;
            //Console.WriteLine(JsonConvert.SerializeObject(result));
            //_result = neteaseSearch.SearchForResults(new TrackMultiArtistMetadata()
            //{
            //    Album = "RUNAWAY",
            //    AlbumArtists = new() { "OneRepublic" },
            //    Artists = new() { "OneRepublic" },
            //    DurationMs = 143264,
            //    Title = "RUNAWAY",
            //}).Result;
            //Console.WriteLine(JsonConvert.SerializeObject(_result));
        }

    }

    internal partial class JsonConvert
    {
        public static string? SerializeObject<T>(T obj)
        {
            if (typeof(T) == typeof(LyricsData)) return System.Text.Json.JsonSerializer.Serialize(obj, JsonContext.Default.LyricsData);
            if (typeof(T) == typeof(Searchers.ISearchResult)) return System.Text.Json.JsonSerializer.Serialize(obj, JsonContext.Default.ISearchResult);
            if (typeof(T) == typeof(List<Searchers.ISearchResult>)) return System.Text.Json.JsonSerializer.Serialize(obj, JsonContext.Default.ListISearchResult);
            return null;
        }

        [System.Text.Json.Serialization.JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
        [System.Text.Json.Serialization.JsonSerializable(typeof(LyricsData))]
        [System.Text.Json.Serialization.JsonSerializable(typeof(Searchers.ISearchResult), GenerationMode = System.Text.Json.Serialization.JsonSourceGenerationMode.Serialization)]
        [System.Text.Json.Serialization.JsonSerializable(typeof(List<Searchers.ISearchResult>), GenerationMode = System.Text.Json.Serialization.JsonSourceGenerationMode.Serialization)]
        internal partial class JsonContext : System.Text.Json.Serialization.JsonSerializerContext { }
    }
}