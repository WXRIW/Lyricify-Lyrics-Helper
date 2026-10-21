# 歌词解析、生成与处理

[文档目录](README.md) · [项目首页](../README.md)

各格式的解析与生成支持情况见项目首页的[歌词格式表](../README.md#支持的歌词格式)。

## 解析

`ParseHelper.ParseLyrics` 将歌词文本解析为 `LyricsData`，输入格式由 `LyricsRawTypes` 表示。

### 自动识别与指定格式

```csharp
using System.IO;
using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;

var rawLyrics = File.ReadAllText("lyrics.txt");
var detectedType = TypeHelper.GetLyricsTypes(rawLyrics);
var lyricsData = ParseHelper.ParseLyrics(rawLyrics);
```

`TypeHelper.GetLyricsTypes` 无法识别输入时返回 `LyricsRawTypes.Unknown`；`ParseHelper.ParseLyrics` 对无法识别或没有对应解析器的类型返回 `null`。

已知原始格式时，也可以显式传入类型：

```csharp
var qrc = File.ReadAllText("lyrics.qrc");
var lyricsData = ParseHelper.ParseLyrics(qrc, LyricsRawTypes.Qrc);
```

`TypeHelper` 还可根据格式名称获取枚举值、显示名称和对应的 `LyricsTypes`：

```csharp
if (TypeHelper.TryParseRawType("QRC", out var rawType))
{
    var displayName = rawType.GetDisplayName();
    var lyricsType = rawType.GetLyricsType();
}
```

### 带封装的歌词数据

格式识别支持 QRC XML、网易云音乐 YRC JSON 和 Apple Music JSON，分别对应 `LyricsRawTypes.QrcFull`、`YrcFull` 和 `AppleJson`。当前 `ParseHelper` 不解析这些封装，应先提取其中的歌词文本：

- QRC XML：提取 QRC 内容，按 `LyricsRawTypes.Qrc` 解析。
- 网易云音乐响应：提取 `yrc.lyric`，按 `LyricsRawTypes.Yrc` 解析。
- Apple Music 响应：提取 TTML 内容，按 `LyricsRawTypes.Ttml` 解析。

### TTML 与 Apple Music 扩展

`TtmlParser` 支持逐行、逐字 TTML，并读取 Apple Music 扩展中的翻译、原生发音、简体中文替换、歌曲元数据、背景人声和演唱者对齐信息。直接调用解析器可设置选项，或接收发音解析的诊断信息：

```csharp
using System;
using System.IO;
using Lyricify.Lyrics.Parsers;

var ttml = File.ReadAllText("lyrics.ttml");
var lyricsData = TtmlParser.Parse(ttml, diagnostic: Console.WriteLine);
var withoutPronunciations = TtmlParser.Parse(ttml, parsePronunciations: false);
```

`parsePronunciations` 默认为 `true`；通过 `ParseHelper` 解析 TTML 时也会读取发音。发音模型可接收其他来源的数据，存储和处理方式见[多语言发音](pronunciation.md)。

## 生成

`GenerateHelper.GenerateString` 将 `LyricsData` 转换为歌词文本，输出格式由 `LyricsTypes` 指定：

```csharp
using System;
using System.IO;
using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Models;

var lyricsData = ParseHelper.ParseLyrics(File.ReadAllText("lyrics.txt"));
if (lyricsData is not null)
{
    var lrc = GenerateHelper.GenerateString(lyricsData, LyricsTypes.Lrc);
    if (lrc is not null)
        Console.WriteLine(lrc);
}
```

可用格式见[歌词格式表](../README.md#支持的歌词格式)。目标类型没有对应生成器或生成结果为空时，返回 `null`。转换结果受目标格式限制，例如 LRC 仅保留逐行时间，无法保存逐字时间。

## 数据模型

`LyricsData` 保存歌词行、文件信息、曲目元数据和词作者等数据。歌词行使用 `ILineInfo` 接口，逐字单元使用 `ISyllableInfo` 接口，时间单位为毫秒。

| 类型 | 用途 |
| --- | --- |
| `LineInfo` | 保存一行文本及可选起止时间，可表示未同步或逐行歌词 |
| `SyllableLineInfo` | 按顺序保存 `Syllables`，文本及起止时间由子项汇总 |
| `FullLineInfo` | 在逐行模型上增加翻译与多语言发音 |
| `FullSyllableLineInfo` | 在逐字行模型上增加翻译与多语言发音汇总 |
| `SyllableInfo` | 保存一个原文单位、起止时间及其多语言发音 |
| `FullSyllableInfo` | 用 `SubItems` 表示一组原文单位，并可保存整组共用的发音 |

`SubLine` 关联背景人声，`LyricsAlignment` 记录对齐信息。`LyricsData.Lines` 允许混用未同步、逐行和逐字行。直接修改 `Syllables` 或 `SubItems` 后，应调用该模型的 `RefreshProperties()` 刷新文本和时间缓存。

## 歌词处理

### 优化与标准化

以下方法位于 `Lyricify.Lyrics.Helpers.Optimization` 命名空间：

| 帮助类或方法 | 用途 |
| --- | --- |
| `Explicit.Clean`、`Explicit.FixExplicit` | 处理或还原 Explicit 歌词文本 |
| `Yrc.StandardizeYrcLyrics` | 标准化 YRC 歌词 |
| `Musixmatch.StandardizeMusixmatchLyrics` | 标准化 Musixmatch 歌词 |
| `InfoLines` | 识别信息行，并统计开头、结尾的信息行数量 |
| `AppleMusic.PrepareLyrics` | 拆分原文单元、分配子项时间，并同步拆分发音或保留共享发音组 |
| `AppleMusic.CapitalizationNormalization` | 规范歌词大小写 |

`AppleMusic.PrepareLyrics` 也可处理其他来源的数据。发音拆分和共享组的规则见[多语言发音](pronunciation.md)。

### 时间偏移与降级

`OffsetHelper.AddOffset` 从原时间中减去 `offset`，适用于逐行和逐字歌词。例如 `offset` 为 `500` 时，`1000ms` 变为 `500ms`；负数则使歌词延后。

`SyncDowngrade.DowngradeToLineSynced` 将逐字歌词转换为逐行歌词，同时处理关联的 `SubLine`。转换后保留行文本、行起止时间、翻译和整行多语言发音。

```csharp
using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Helpers.Optimization;

if (lyricsData?.Lines is { } lines)
{
    OffsetHelper.AddOffset(lines, 500);
    lines.DowngradeToLineSynced();
}
```

上述操作会修改传入数据。如需保留原始数据，应先创建独立副本。

## 解密与通用工具

`Lyricify.Lyrics.Decrypter.Qrc.Decrypter.DecryptLyrics` 和 `Lyricify.Lyrics.Decrypter.Krc.Decrypter.DecryptLyrics` 分别解密 QRC、KRC 歌词字符串。相应目录中的 `Helper` 提供歌词获取方法，在线接口的用法见[搜索与在线接口](providers.md)。

其他工具包括：

- `InstrumentalHelper`：判断歌词是否为纯音乐标记。
- `ChineseHelper`、`ChineseConverter`：中文简繁转换。
- `StringHelper`：字符串比较、空白处理和时间字符串转换。
- `MathHelper`：可空数值比较和范围判断。

字符串和数学工具位于 `Lyricify.Lyrics.Helpers.General` 命名空间。更多示例见仓库中的 `Lyricify.Lyrics.Demo/Program.cs`。
