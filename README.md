# Lyricify Lyrics Helper

Lyricify 家族中的 .NET 歌词工具库，为 Lyricify 歌词相关功能竭力打造，提供歌词解析、生成、搜索、解密和优化处理。

## 主要功能

### 歌词格式

| 格式 | 解析 | 生成 | 说明 |
| --- | :---: | :---: | --- |
| Lyricify Syllable | ✓ | ✓ | Lyricify 逐字歌词 |
| Lyricify Lines | ✓ | ✓ | Lyricify 逐行歌词 |
| LRC | ✓ | ✓ | 标准逐行歌词 |
| QRC | ✓ | ✓ | QQ 音乐逐字歌词 |
| KRC | ✓ | ✓ | 酷狗逐字歌词 |
| YRC | ✓ | ✓ | 网易云音乐逐字歌词 |
| TTML | ✓ | — | 逐行、逐字；支持 Apple Music 扩展 |
| Spotify (JSON) | ✓ | — | 未同步、逐行或逐字歌词 |
| Musixmatch (JSON) | ✓ | — | 未同步、逐行或逐字歌词 |

`ParseHelper` 和 `GenerateHelper` 分别提供统一的解析、生成入口；`TypeHelper` 用于自动识别格式、转换类型以及获取格式的显示名称。

格式识别还支持 QRC (XML)、网易云音乐完整 YRC (JSON) 和 Apple Music (JSON) 等原始封装类型。

### 歌曲搜索与在线接口

`SearchHelper` 支持下表中的全部来源，可根据标题、艺人、专辑和时长查找最匹配的歌曲。

| 来源 | 歌词能力 | 其他常用接口 | 凭据要求 |
| --- | --- | --- | --- |
| QQ 音乐 | 获取歌词及翻译 | 歌曲、专辑、歌单和播放链接 | 无需配置 |
| 网易云音乐 | 获取 LRC、YRC 等歌词数据 | 歌曲、专辑和歌单 | 无需配置 |
| 酷狗音乐 | 搜索歌词并获取 KRC | 歌曲搜索 | 无需配置 |
| 汽水音乐 | 从曲目详情获取歌词信息 | 曲目详情 | 无需配置 |
| Apple Music | 获取 TTML 歌词 | 曲目搜索 | 获取歌词必须提供 Media User Token；Access Token 自动获取 |
| Musixmatch | 获取逐字、逐行或未同步歌词及翻译 | 曲目匹配 | User Token 自动获取，也可手动设置 |
| Spotify | 获取未同步、逐行或逐字歌词 | 曲目搜索 | 必须提供 `sp_dc` Cookie |
| LRCLIB | 获取同步或纯文本歌词 | 按曲目信息搜索、按 ID 查询 | 无需配置 |

- 已有歌曲信息、需要寻找对应平台曲目时，使用 `SearchHelper`
- 已知来源、曲目 ID 或需要调用特定平台能力时，使用 `ProviderHelper`

在线接口依赖对应服务的可用性，返回结果也可能受到账号权限和地区限制。

### 歌词优化

- 处理或还原 Explicit 歌词
- 标准化 YRC、Musixmatch 歌词
- 识别并处理信息行（标题行）
- 解析 Apple Music TTML 扩展中的翻译、简体中文替换、歌曲元数据、背景人声和演唱者对齐信息

### 时间轴与结构处理

- 为逐行、逐字歌词添加时间偏移
- 将逐字歌词降级为逐行歌词等较低同步级别
- 使用统一模型表示未同步、逐行、逐字和混合同步歌词

### 解密与通用工具

- QRC、KRC 歌词解密与获取帮助
- 中文简繁转换
- 字符串和数学帮助方法

## 快速开始

### 引用项目

在解决方案目录中添加项目引用：

```powershell
dotnet add path/to/YourProject.csproj reference Lyricify.Lyrics.Helper/Lyricify.Lyrics.Helper.csproj
```

### 解析与转换

```csharp
using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;

var rawLyrics = File.ReadAllText("lyrics.txt");
var lyricsData = ParseHelper.ParseLyrics(rawLyrics); // 自动识别格式

if (lyricsData is not null)
{
    var lrc = GenerateHelper.GenerateString(lyricsData, LyricsTypes.Lrc);
    Console.WriteLine(lrc);
}
```

已知原始格式时，也可以显式传入类型：

```csharp
var lyricsData = ParseHelper.ParseLyrics(rawLyrics, LyricsRawTypes.Qrc);
```

### 搜索与获取歌词

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

其他在线来源可通过 `ProviderHelper` 中相应的 `QQMusicApi`、`NeteaseApi`、`KugouApi`、`SodaMusicApi`、`AppleMusicApi`、`MusixmatchApi` 和 `SpotifyApi` 访问。

Musixmatch 默认使用 Android API，也可以在创建实例时通过配置委托选择桌面 API，或注入自己的请求发送函数：

```csharp
var mobileApi = new Lyricify.Lyrics.Providers.Web.Musixmatch.Api();
var desktopApi = new Lyricify.Lyrics.Providers.Web.Musixmatch.Api(
    options => options.UseDesktop());

var searcher = new MusixmatchSearcher(desktopApi);
```

## 项目结构

```text
Lyricify.Lyrics.Helper/
├─ Decrypter/        # QRC、KRC 解密
├─ Generators/       # 歌词生成
├─ Helpers/          # 统一入口、歌词优化与通用帮助类
├─ Models/           # 歌词、时间轴和曲目元数据模型
├─ Parsers/          # 歌词解析
├─ Providers/Web/    # 在线服务接口与响应模型
└─ Searchers/        # 歌曲搜索与匹配

Lyricify.Lyrics.Demo/
├─ Program.cs        # 使用示例
└─ RawLyrics/        # 示例歌词
```

## Lyricify 家族

Lyricify Lyrics Helper 是 Lyricify 家族的开源成员之一，专注于为 Lyricify 系列应用及其他项目提供可复用的歌词处理能力。

### Lyricify 主仓库

- [Lyricify App](https://github.com/WXRIW/Lyricify-App)：Lyricify 系列应用的主仓库，包含 Lyricify 4、Lyricify Fusion、Lyricify Mobile 等产品的介绍、下载与使用指南。

### Lyricify 系列的其他开源项目

- [Lyricify Backgrounds](https://github.com/WXRIW/Lyricify-Backgrounds)：面向 Lyricify 的可复用动态背景渲染器，提供跨框架接口以及 WPF、WinUI 实现。
- [Lyricify Lines Creator](https://github.com/WXRIW/Lyricify-Lines-Creator)：Lyricify Lines 逐行歌词打轴工具，同时支持输出 LRC 歌词。

## 许可证

本项目基于 [Apache License 2.0](LICENSE) 开源。

欢迎开发者在自己的项目中引用本库。使用、复制、修改或分发本项目代码时，请遵守 Apache License 2.0，并履行许可证规定的相关义务。

如果希望对本项目进行改进、翻译或衍生开发，推荐直接 fork 本仓库并在此基础上继续维护，而非另行创建项目；同时建议保留原仓库名称或 Lyricify 标识，以便识别项目来源。

## 感谢与支持

特别感谢 [@cnbluefire](https://github.com/cnbluefire) 和 [@Raspberry Kan](https://github.com/Raspberry-Monster) 提供的帮助与支持。

#### 感谢以下第三方代码
- [LyricParser](https://github.com/HyPlayer/LyricParser)（MIT License）
- [163MusicLyrics](https://github.com/jitwxs/163MusicLyrics)（Apache-2.0 License）
