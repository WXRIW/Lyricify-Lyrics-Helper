using Lyricify.Lyrics.Models;
using Lyricify.Lyrics.Parsers.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lyricify.Lyrics.Parsers
{
    public static class MusixmatchParser
    {
        public static LyricsData? Parse(string rawJson)
        {
            return Parse(rawJson, false);
        }

        /// <param name="ignoreSyllable">忽略逐字歌词</param>
        public static LyricsData? Parse(string rawJson, bool ignoreSyllable)
        {
            JsonDocument? jsonDoc = null;

            try
            {
                jsonDoc = JsonDocument.Parse(rawJson, new JsonDocumentOptions()
                {
                    AllowTrailingCommas = true,
                });
            }
            catch
            {
                return null;
            }

            var root = (JsonElementResult)jsonDoc.RootElement;

            var calls = root["message"]["body"]["macro_calls"];
            if (calls.ValueKind != JsonValueKind.Object) return null;

            static bool CheckHeader200(JsonElementResult getObj)
            {
                var obj = getObj["message"]["header"]["status_code"];
                if (obj.ValueKind != JsonValueKind.Number) return false;
                if (obj.Value.GetInt32() != 200) return false;
                return true;
            }

            var track_get = calls["track.richsync.get"];
            if (!ignoreSyllable && CheckHeader200(track_get))
            {
                var lyrics = track_get["message"]["body"]["richsync"]["richsync_body"].GetString();

                if (!string.IsNullOrEmpty(lyrics) && Helpers.JsonConvert.DeserializeObject<List<RichSyncedLine>>(lyrics) is List<RichSyncedLine> list)
                {
                    var lines = new List<ILineInfo>();
                    foreach (var line in list)
                    {
                        var syllables = new List<SyllableInfo>();
                        var start = (int)(line.TimeStart * 1000);
                        for (int i = 0; i < line.Words.Count; i++)
                        {
                            syllables.Add(new()
                            {
                                StartTime = start + (int)(line.Words[i].Position * 1000),
                                EndTime = i + 1 < line.Words.Count ? start + (int)(line.Words[i + 1].Position * 1000) : (int)(line.TimeEnd * 1000),
                                Text = line.Words[i].Chars,
                            });
                        }
                        lines.Add(new SyllableLineInfo()
                        {
                            Syllables = syllables.Cast<ISyllableInfo>().ToList(),
                        });
                    }

                    var lyricsData = new LyricsData
                    {
                        File = new(),
                        Lines = lines,
                    };
                    lyricsData.File.Type = LyricsTypes.Musixmatch;
                    lyricsData.File.SyncTypes = SyncTypes.SyllableSynced;
                    return lyricsData;
                }
            }

            track_get = calls["track.subtitles.get"];
            if (CheckHeader200(track_get))
            {
                var list = track_get["message"]["body"]["subtitle_list"];
                if (list.HasValue && list.ValueKind == JsonValueKind.Array && list.Value.GetArrayLength() > 0)
                {
                    var subtitle = list[0]["subtitle"]["subtitle_body"].GetString();
                    if (!string.IsNullOrEmpty(subtitle))
                    {
                        var lines = LrcParser.ParseLyrics(subtitle);
                        var lyricsData = new LyricsData
                        {
                            File = new(),
                            Lines = lines,
                        };
                        lyricsData.File.Type = LyricsTypes.Musixmatch;
                        lyricsData.File.SyncTypes = SyncTypes.LineSynced;
                        return lyricsData;
                    }
                }
            }

            track_get = calls["track.lyrics.get"];
            if (CheckHeader200(track_get))
            {
                var lyrics = track_get["message"]["body"]["lyrics"]["lyrics_body"].GetString();

                if (!string.IsNullOrEmpty(lyrics))
                {
                    var list = lyrics.Trim()
                        .Split('\n')
                        .Select(line => new LineInfo { Text = line })
                        .Cast<ILineInfo>()
                        .ToList();

                    var lyricsData = new LyricsData
                    {
                        File = new(),
                        Lines = list,
                    };
                    lyricsData.File.Type = LyricsTypes.Musixmatch;
                    lyricsData.File.SyncTypes = SyncTypes.Unsynced;
                    return lyricsData;
                }
            }

            return null;
        }

        private struct JsonElementResult
        {
            internal JsonElementResult(JsonElement element)
            {
                HasValue = element.ValueKind != JsonValueKind.Undefined;
                Value = element;
            }

            public readonly JsonElement Value { get; }

            public readonly bool HasValue { get; }

            public readonly JsonValueKind ValueKind => HasValue ? Value.ValueKind : JsonValueKind.Undefined;

            public readonly JsonElementResult this[string property]
            {
                get
                {
                    if (HasValue && Value.TryGetProperty(property, out var result))
                    {
                        return new JsonElementResult(result);
                    }
                    return default;
                }
            }

            public readonly JsonElementResult this[int index]
            {
                get
                {
                    if (HasValue && ValueKind == JsonValueKind.Array)
                    {
                        if (index < 0 || index >= Value.GetArrayLength())
                        {
                            ThrowIndexOutOfRangeException();
                        }
                        using (var enumerator = Value.EnumerateArray())
                        {
                            do
                            {
                                enumerator.MoveNext();
                                index--;
                            } while (index >= 0);
                            return new JsonElementResult(enumerator.Current);
                        }
                    }

                    ThrowInvalidOperationException();
                    return default;

                    [DoesNotReturn]
                    static void ThrowInvalidOperationException()
                    {
                        throw new InvalidOperationException();
                    }

                    [DoesNotReturn]
                    static void ThrowIndexOutOfRangeException()
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public readonly string? GetString()
            {
                if (HasValue && Value.ValueKind == JsonValueKind.String) return Value.GetString();
                return null;
            }

            public static implicit operator JsonElement?(JsonElementResult result) => result.HasValue ? result.Value : null;

            public static implicit operator JsonElementResult(JsonElement element) => new JsonElementResult(element);

            public static implicit operator JsonElementResult(JsonElement? element) => element.HasValue ? new JsonElementResult(element.Value) : default;
        }

    }

}