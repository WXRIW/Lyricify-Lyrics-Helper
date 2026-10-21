# 多语言发音

[文档目录](README.md) · [项目首页](../README.md)

本库支持为逐行、逐字歌词保存多语言发音。目前，`TtmlParser` 可读取 Apple Music TTML 中的原生发音。其他来源的发音数据也可以直接写入模型。

## 写入与读取

发音以完整语言标签为键，按顺序保存文本：`Dictionary<string, List<string>>`。例如，“今生”的发音可写为 `zh-Latn → ["jīn ", "shēng"]`。

```csharp
using Lyricify.Lyrics.Helpers;
using Lyricify.Lyrics.Helpers.Optimization;
using Lyricify.Lyrics.Models;

var syllable = new SyllableInfo("今生", 1000, 3000);
syllable.Pronunciations["zh-Latn"] = new() { "jīn ", "shēng" };
var line = new FullSyllableLineInfo(new SyllableLineInfo(new[] { syllable }));

Console.WriteLine(line.GetPronunciation());          // 默认语言
Console.WriteLine(line.GetPronunciation("zh-Latn")); // 指定完整语言标签
AppleMusic.PrepareLyrics(line);                     // 同步拆分原文及发音
```

`GetPronunciations()` 扩展方法通过现有的 `ILineInfo`、`ISyllableInfo` 接口读取发音，不要求第三方接口实现增加成员。`GetPronunciation()` 按顺序连接文本，保留原有空白，不额外插入空格。指定语言不存在时返回 `null`；该语言存在但发音为空时返回空字符串。

## 模型与共享发音组

| 模型 | 存储与读取 |
| --- | --- |
| `SyllableInfo` | 可修改的 `Pronunciations`，对应当前原文单位 |
| `FullLineInfo` | 可修改的 `Pronunciations`，对应整行原文 |
| `FullSyllableInfo` | `GroupPronunciations` 保存整组共用的发音；`Pronunciations` 提供只读汇总 |
| `FullSyllableLineInfo` | `Pronunciations` 提供只读汇总；无法关联原文片段的整行发音保存在内部 |

汇总结果中的字典和列表均为只读。`FullSyllableInfo` 按语言分别汇总：该语言有组级发音时，使用 `GroupPronunciations`；否则按顺序连接 `SubItems` 的发音。因此，同一组中的不同语言可以分别对应整组或各个子项。

例如，“見るよ”中，“見”和“る”共用 `miru`，“よ”单独对应 `yo`：

```csharp
var group = new FullSyllableInfo(new[]
{
    new SyllableInfo("見", 1000, 1500),
    new SyllableInfo("る", 1500, 2000),
});
group.GroupPronunciations["ja-Latn"] = new() { "miru" };

var last = new SyllableInfo("よ", 2000, 2500);
last.Pronunciations["ja-Latn"] = new() { "yo" };
var line = new FullSyllableLineInfo(
    new SyllableLineInfo(new ISyllableInfo[] { group, last }));
```

`miru` 属于整个“見る”组，“見”和“る”保留各自的时间。组级发音不重复写入子项；合并相邻单元时，也保留该组的边界。

## 默认语言与旧字符串属性

读取行发音时，优先使用模型记录的默认语言；未记录或该语言不存在时，优先选择带 `Latn` 标签的语言，再取集合中的首种语言。也可通过完整标签读取指定语言，例如 `line.GetPronunciation("ja-Latn")`。

`FullLineInfo` 和 `FullSyllableLineInfo` 保留原有的 `string? Pronunciation` 属性。显式赋值后，该属性返回赋值内容；设置为 `null` 后，恢复返回默认语言的汇总结果：

```csharp
line.Pronunciation = "自定义显示文本";
line.Pronunciation = null; // 恢复根据发音集合计算
```

## 拆分与结构处理

`AppleMusic.PrepareLyrics` 按原有规则拆分原文并分配时间，同时处理对应的发音：

- “今生”关联 `["jīn ", "shēng"]` 后，可拆成“今／生”并按顺序分配发音。
- “世界”关联 `["shì jiè"]` 后，可按明确的空白边界拆成 `["shì ", "jiè"]`。
- 发音项数量与原文子项数量一致，或按空白分词后数量一致时，按顺序分配；不按字母长度切分。
- “見る”的 `miru` 无法继续分配，保留在 `FullSyllableInfo.GroupPronunciations` 中，原文仍拆成“見／る”。

各语言独立处理。裁剪和合并时复制关联发音；降级为逐行歌词时，按原文顺序汇总各语言发音。重复调用 Prepare 不会重复拼接发音。

发音不另设时间戳，使用所属原文单位的时间；偏移处理只需调整原文时间。

## JSON 保存

使用 `Newtonsoft.Json` 序列化时，发音集合、组级归属、未关联的整行发音及默认语言均可保存。存在多语言数据时，`Pronunciation` 仅序列化显式赋值，避免读回后将自动汇总结果固定为覆盖值。未使用多语言数据时，不输出相关新增字段。

`LyricsData.Lines` 和 `SyllableLineInfo.Syllables` 使用接口类型，反序列化时需要按应用的数据约定恢复为具体模型类型。

## 从 TTML 读取发音

`TtmlParser.Parse` 的 `parsePronunciations` 参数默认为 `true`；通过 `ParseHelper` 解析 TTML 时也默认读取发音。

```csharp
using Lyricify.Lyrics.Parsers;

var ttml = File.ReadAllText("lyrics.ttml");
var lyrics = TtmlParser.Parse(ttml, diagnostic: Console.WriteLine);
var withoutPronunciations = TtmlParser.Parse(ttml, parsePronunciations: false);
```

解析阶段保留原文的分段和时间，不调用 Prepare。发音时标明确对应多个相邻原文单位时，解析器建立共享发音组；原生发音时标仅用于关联，不写入公共模型。

对于已匹配的歌词行，无法唯一归属或需要交叉分组的发音按整行保存。可通过 `diagnostic` 回调查看关联诊断。

TTML 的默认发音语言依次按原文语言匹配、`Latn` 标签和发音轨顺序确定，并记录到模型中。未记录默认语言时，采用[默认语言与旧字符串属性](#默认语言与旧字符串属性)中的通用规则。
