using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Lyricify.Lyrics.Helpers
{
    internal static partial class JsonConvert
    {
        public static T? DeserializeObject<T>(string json)
        {
            return JsonSerializer.Deserialize(json, GetJsonContext<T>());
        }

        public static string SerializeObject<T>(T? obj)
        {
            return JsonSerializer.Serialize(obj, GetJsonContext<T?>());
        }

        private static JsonTypeInfo<T> GetJsonContext<T>()
        {
            if (typeof(T) == typeof(List<string>)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext.Default.ListString;
            else if (typeof(T) == typeof(Dictionary<string, string>)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext.Default.DictionaryStringString;

            else if (typeof(T) == typeof(Decrypter.Krc.KugouLyricsResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Decrypter_Krc.Default.KugouLyricsResponse;
            else if (typeof(T) == typeof(Decrypter.Krc.KugouTranslation)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Decrypter_Krc.Default.KugouTranslation;

            else if (typeof(T) == typeof(Decrypter.Qrc.QqLyricsResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Decrypter_Qrc.Default.QqLyricsResponse;
            else if (typeof(T) == typeof(Decrypter.Qrc.SongResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Decrypter_Qrc.Default.SongResponse;

            else if (typeof(T) == typeof(Providers.Web.Kugou.SearchSongResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Kugou.Default.SearchSongResponse;
            else if (typeof(T) == typeof(Providers.Web.Kugou.SearchLyricsResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Kugou.Default.SearchLyricsResponse;

            else if (typeof(T) == typeof(Providers.Web.Musixmatch.GetTokenResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Musixmatch.Default.GetTokenResponse;
            else if (typeof(T) == typeof(Providers.Web.Musixmatch.GetTrackResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Musixmatch.Default.GetTrackResponse;
            else if (typeof(T) == typeof(Providers.Web.Musixmatch.GetTranslationsResponse)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Musixmatch.Default.GetTranslationsResponse;

            else if (typeof(T) == typeof(Providers.Web.Netease.SearchResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.SearchResult;
            else if (typeof(T) == typeof(Providers.Web.Netease.EapiSearchResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.EapiSearchResult;
            else if (typeof(T) == typeof(Providers.Web.Netease.SongUrls)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.SongUrls;
            else if (typeof(T) == typeof(Providers.Web.Netease.LyricResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.LyricResult;
            else if (typeof(T) == typeof(Providers.Web.Netease.PlaylistResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.PlaylistResult;
            else if (typeof(T) == typeof(Providers.Web.Netease.AlbumResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.AlbumResult;
            else if (typeof(T) == typeof(Providers.Web.Netease.Song)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.Song;
            else if (typeof(T) == typeof(Providers.Web.Netease.DetailResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_Netease.Default.DetailResult;

            else if (typeof(T) == typeof(Providers.Web.QQMusic.MusicFcgApiResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.MusicFcgApiResult;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.MusicFcgApiAlternativeResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.MusicFcgApiAlternativeResult;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.AlbumResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.AlbumResult;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.PlaylistResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.PlaylistResult;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.LyricResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.LyricResult;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.SingerSongResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.SingerSongResult;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.ToplistResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.ToplistResult;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.AlbumSongListResult)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic.Default.AlbumSongListResult;

            else if (typeof(T) == typeof(List<Parsers.Models.RichSyncedLine>)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Parsers_Models.Default.ListRichSyncedLine;
            else if (typeof(T) == typeof(Parsers.Models.RichSyncedLine)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Parsers_Models.Default.RichSyncedLine;

            else if (typeof(T) == typeof(Parsers.Models.Spotify.SpotifyColorLyrics)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Parsers_Models_Spotify.Default.SpotifyColorLyrics;

            else if (typeof(T) == typeof(Parsers.Models.Yrc.CreditsInfo)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Parsers_Models_Yrc.Default.CreditsInfo;

            else if (typeof(T) == typeof(Providers.Web.QQMusic.Api.SearchRequestModel)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic_Api.Default.SearchRequestModel;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.Api.GetAlbumSongListRequestModel)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic_Api.Default.GetAlbumSongListRequestModel;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.Api.GetSingerSongsRequestModel)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic_Api.Default.GetSingerSongsRequestModel;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.Api.GetToplistRequestModel)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic_Api.Default.GetToplistRequestModel;
            else if (typeof(T) == typeof(Providers.Web.QQMusic.Api.GetSongLinkRequestModel)) return (JsonTypeInfo<T>)(JsonTypeInfo)JsonContext_Providers_Web_QQMusic_Api.Default.GetSongLinkRequestModel;

            return ThrowException();

            [DoesNotReturn]
            static JsonTypeInfo<T> ThrowException()
            {
                throw new InvalidOperationException(typeof(T).FullName);
            }
        }


        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(List<string>))]
        [JsonSerializable(typeof(Dictionary<string, string>))]
        internal partial class JsonContext : JsonSerializerContext { }


        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Decrypter.Krc.KugouLyricsResponse))]
        [JsonSerializable(typeof(Decrypter.Krc.KugouTranslation))]
        internal partial class JsonContext_Decrypter_Krc : JsonSerializerContext { }


        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Decrypter.Qrc.QqLyricsResponse))]
        [JsonSerializable(typeof(Decrypter.Qrc.SongResponse))]
        internal partial class JsonContext_Decrypter_Qrc : JsonSerializerContext { }


        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Providers.Web.Kugou.SearchSongResponse))]
        [JsonSerializable(typeof(Providers.Web.Kugou.SearchLyricsResponse))]
        internal partial class JsonContext_Providers_Web_Kugou : JsonSerializerContext { }


        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Providers.Web.Musixmatch.GetTokenResponse))]
        [JsonSerializable(typeof(Providers.Web.Musixmatch.GetTrackResponse))]
        [JsonSerializable(typeof(Providers.Web.Musixmatch.GetTranslationsResponse))]
        internal partial class JsonContext_Providers_Web_Musixmatch : JsonSerializerContext { }


        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Providers.Web.Netease.SearchResult))]
        [JsonSerializable(typeof(Providers.Web.Netease.EapiSearchResult))]
        [JsonSerializable(typeof(Providers.Web.Netease.SongUrls))]
        [JsonSerializable(typeof(Providers.Web.Netease.LyricResult))]
        [JsonSerializable(typeof(Providers.Web.Netease.PlaylistResult))]
        [JsonSerializable(typeof(Providers.Web.Netease.AlbumResult))]
        [JsonSerializable(typeof(Providers.Web.Netease.Song))]
        [JsonSerializable(typeof(Providers.Web.Netease.DetailResult))]
        internal partial class JsonContext_Providers_Web_Netease : JsonSerializerContext { }


        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Providers.Web.QQMusic.MusicFcgApiResult))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.MusicFcgApiAlternativeResult))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.AlbumResult))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.PlaylistResult))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.LyricResult))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.SingerSongResult))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.ToplistResult))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.AlbumSongListResult))]
        internal partial class JsonContext_Providers_Web_QQMusic : JsonSerializerContext { }

        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(List<Lyricify.Lyrics.Parsers.Models.RichSyncedLine>))]
        internal partial class JsonContext_Parsers_Models : JsonSerializerContext { }

        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Lyricify.Lyrics.Parsers.Models.Spotify.SpotifyColorLyrics))]
        internal partial class JsonContext_Parsers_Models_Spotify : JsonSerializerContext { }

        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Lyricify.Lyrics.Parsers.Models.Yrc.CreditsInfo))]
        internal partial class JsonContext_Parsers_Models_Yrc : JsonSerializerContext { }

        [JsonSourceGenerationOptions(
            AllowTrailingCommas = true,
            DictionaryKeyPolicy = JsonKnownNamingPolicy.Unspecified,
            GenerationMode = JsonSourceGenerationMode.Default,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            UseStringEnumConverter = true,
            WriteIndented = false)]
        [JsonSerializable(typeof(Providers.Web.QQMusic.Api.SearchRequestModel))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.Api.GetAlbumSongListRequestModel))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.Api.GetSingerSongsRequestModel))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.Api.GetToplistRequestModel))]
        [JsonSerializable(typeof(Providers.Web.QQMusic.Api.GetSongLinkRequestModel))]
        internal partial class JsonContext_Providers_Web_QQMusic_Api : JsonSerializerContext { }
    }

    public class ULongToStringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeof(ulong) == typeToConvert) return $"{reader.GetUInt64()}";

            return null;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            var ulongValue = ulong.Parse(value);
            writer.WriteNumberValue(ulongValue);
        }
    }
}
