# 文档

[项目首页](../README.md) · [快速开始](../README.md#快速开始)

## 使用说明

- [歌词解析与处理](lyrics.md)：格式识别、解析、生成、数据模型和时间处理。
- [搜索与在线接口](providers.md)：歌词来源、访问凭据、搜索和获取示例。
- [多语言发音](pronunciation.md)：发音读写、共享组、语言选择、JSON 和 TTML 解析。

## 项目结构

```text
Lyricify.Lyrics.Helper/
├─ Decrypter/        # QRC、KRC 解密
├─ Generators/       # 歌词生成
├─ Helpers/          # 解析入口、歌词优化和帮助类
├─ Models/           # 歌词、时间轴、发音和曲目元数据模型
├─ Parsers/          # 歌词解析
├─ Providers/Web/    # 在线服务接口与响应模型
└─ Searchers/        # 歌曲搜索与匹配

Lyricify.Lyrics.Demo/
├─ Program.cs        # 使用示例
└─ RawLyrics/        # 示例歌词

docs/
├─ README.md         # 文档目录
├─ lyrics.md         # 歌词解析与处理
├─ providers.md      # 搜索与在线接口
└─ pronunciation.md  # 多语言发音
```
