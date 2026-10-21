# 歌曲搜索与在线接口

[文档目录](README.md) · [项目首页](../README.md)

`SearchHelper` 可根据标题、艺人、专辑和时长查找最匹配的歌曲。已知来源、曲目 ID 或需要调用特定平台能力时，使用 `ProviderHelper`。各来源支持的功能见首页的[歌曲搜索与在线接口](../README.md#歌曲搜索与在线接口)。

在线接口依赖对应服务的可用性，返回结果也可能受到账号权限和地区限制。

## 访问凭据

| 来源 | 配置要求 |
| --- | --- |
| QQ 音乐、网易云音乐、酷狗音乐、汽水音乐、LRCLIB | 无需配置 |
| Apple Music | 获取歌词必须提供 Media User Token；Access Token 自动获取 |
| Musixmatch | User Token 自动获取，也可手动设置 |
| Spotify | 必须提供 `sp_dc` Cookie |

## 搜索与获取歌词

以下示例搜索 LRCLIB 曲目，并按曲目信息获取歌词。`TrackMultiArtistMetadata.DurationMs` 使用毫秒，传入 LRCLIB API 时需换算为秒。

```csharp
using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;
using Lyricify.Lyrics.Searchers.Helpers;
using SearchSource = Lyricify.Lyrics.Searchers.Searchers;

var track = new TrackMultiArtistMetadata
{
    Title = "RUNAWAY",
    Artists = new() { "OneRepublic" },
    Album = "RUNAWAY",
    AlbumArtists = new() { "OneRepublic" },
    DurationMs = 143264,
};

var match = await SearchHelper.Search(
    track,
    SearchSource.LRCLIB,
    CompareHelper.MatchType.Medium);

var lyrics = await ProviderHelper.LRCLIBApi.Get(
    track.Title,
    track.Artist!,
    track.Album,
    track.DurationMs / 1000d);

Console.WriteLine(lyrics?.SyncedLyrics);
```

`ProviderHelper` 提供以下 API 入口：

| 来源 | 入口 |
| --- | --- |
| QQ 音乐 | `ProviderHelper.QQMusicApi` |
| 网易云音乐 | `ProviderHelper.NeteaseApi` |
| 酷狗音乐 | `ProviderHelper.KugouApi` |
| 汽水音乐 | `ProviderHelper.SodaMusicApi` |
| Apple Music | `ProviderHelper.AppleMusicApi` |
| Musixmatch | `ProviderHelper.MusixmatchApi` |
| Spotify | `ProviderHelper.SpotifyApi` |
| LRCLIB | `ProviderHelper.LRCLIBApi` |

歌词解析和格式转换见[歌词格式与处理](lyrics.md)，发音数据的使用方式见[多语言发音](pronunciation.md)。

## Musixmatch API 配置

Musixmatch 默认使用 Android API。创建实例时，可通过配置委托切换为桌面 API：

```csharp
using Lyricify.Lyrics.Searchers;

var mobileApi = new Lyricify.Lyrics.Providers.Web.Musixmatch.Api();
var desktopApi = new Lyricify.Lyrics.Providers.Web.Musixmatch.Api(
    options => options.UseDesktop());

var searcher = new MusixmatchSearcher(desktopApi);
```

如需自定义请求发送方式，可在配置委托中设置 `ApiOptions.SendAsync`。
