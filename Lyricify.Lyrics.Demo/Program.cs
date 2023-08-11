﻿using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;

namespace Lyricify.Lyrics.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* Parsers Demo */

            LyricsData? lyricsData;

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifySyllableDemo.txt"), LyricsRawTypes.LyricifySyllable);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LsMixQrcDemo.txt"), LyricsRawTypes.LyricifySyllable);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LyricifyLinesDemo.txt"), LyricsRawTypes.LyricifyLines);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/LrcDemo.txt"), LyricsRawTypes.Lrc);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/QrcDemo.txt"), LyricsRawTypes.Qrc);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/KrcDemo.txt"), LyricsRawTypes.Krc);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/YrcDemo.txt"), LyricsRawTypes.Yrc);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));
            //Helpers.Optimization.Yrc.StandardizeYrcLyrics(lyricsData!.Lines!); // 优化 YRC 歌词
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyDemo.txt"), LyricsRawTypes.Spotify);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifySyllableDemo.txt"), LyricsRawTypes.Spotify);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            //lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("RawLyrics/SpotifyUnsyncedDemo.txt"), LyricsRawTypes.Spotify);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(lyricsData, Newtonsoft.Json.Formatting.Indented));

            /* Type Detector Demo */

            //Console.WriteLine(Helpers.Types.Lrc.IsLrc(File.ReadAllText("RawLyrics/LrcDemo.txt")));
            //Console.WriteLine(Helpers.Types.Lrc.IsLrc(File.ReadAllText("RawLyrics/QrcDemo.txt")));
        }
    }
}