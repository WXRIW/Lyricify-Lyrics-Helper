# Lyricify Lyrics Helper

Lyricify 家族中的 .NET 歌词工具库，为 Lyricify 歌词相关功能竭力打造，提供歌词解析、生成、搜索、解密和优化处理。

## 主要功能

- 歌词格式识别、解析和生成
- 歌曲搜索、匹配与歌词获取
- 逐行、逐字歌词处理，支持翻译和多语言发音
- 歌词优化、时间偏移、解密与中文简繁转换

## 支持的歌词格式

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

`ParseHelper` 和 `GenerateHelper` 分别提供统一的解析、生成入口，详细用法见[歌词解析与处理](docs/lyrics.md)。

## 歌曲搜索与在线接口

`SearchHelper` 支持下表中的全部来源，可根据标题、艺人、专辑和时长查找最匹配的歌曲。

| 来源 | 歌词能力 | 其他常用接口 |
| --- | --- | --- |
| QQ 音乐 | 获取歌词及翻译 | 歌曲、专辑、歌单和播放链接 |
| 网易云音乐 | 获取 LRC、YRC 等歌词数据 | 歌曲、专辑和歌单 |
| 酷狗音乐 | 搜索歌词并获取 KRC | 歌曲搜索 |
| 汽水音乐 | 从曲目详情获取歌词信息 | 曲目详情 |
| Apple Music | 获取 TTML 歌词 | 曲目搜索 |
| Musixmatch | 获取逐字、逐行或未同步歌词及翻译 | 曲目匹配 |
| Spotify | 获取未同步、逐行或逐字歌词 | 曲目搜索 |
| LRCLIB | 获取同步或纯文本歌词 | 按曲目信息搜索、按 ID 查询 |

访问凭据和调用示例见[搜索与在线接口](docs/providers.md)。

## 快速开始

在解决方案目录中添加项目引用：

```powershell
dotnet add path/to/YourProject.csproj reference Lyricify.Lyrics.Helper/Lyricify.Lyrics.Helper.csproj
```

读取歌词文件并转换为 LRC：

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

## 文档

- [文档目录与项目结构](docs/README.md)
- [歌词解析与处理](docs/lyrics.md)
- [搜索与在线接口](docs/providers.md)
- [多语言发音](docs/pronunciation.md)

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

### 感谢以下第三方代码

- [LyricParser](https://github.com/HyPlayer/LyricParser)（MIT License）
- [163MusicLyrics](https://github.com/jitwxs/163MusicLyrics)（Apache-2.0 License）
