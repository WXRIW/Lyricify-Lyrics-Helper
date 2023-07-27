using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LyricsData? lyricsData;

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LrcDemo.txt"), LyricsRawTypes.Lrc);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/KrcDemo.txt"), LyricsRawTypes.Krc);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/YrcDemo.txt"), LyricsRawTypes.Yrc);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyDemo.txt"), LyricsRawTypes.Spotify);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifySyllableDemo.txt"), LyricsRawTypes.Spotify);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyUnsyncedDemo.txt"), LyricsRawTypes.Spotify);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));
        }
    }
}