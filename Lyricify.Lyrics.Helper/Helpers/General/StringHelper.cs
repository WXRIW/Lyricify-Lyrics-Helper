using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace Lyricify.Lyrics.Helpers.General
{
    public static class StringHelper
    {
        /// <summary>
        /// 移除两个及以上的空格
        /// </summary>
        public static string RemoveDuoSpaces(this string str)
        {
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }

        /// <summary>
        /// 移除三个及以上的空格
        /// </summary>
        public static string RemoveTripleSpaces(this string str)
        {
            while (str.Contains("   "))
            {
                str = str.Replace("   ", "  ");
            }
            return str;
        }

        /// <summary>
        /// 修复 I 相关单词的大小写问题
        /// </summary>
        public static string FixIWords(this string str)
        {
            str = str.Replace(" i ", " I ")
                     .Replace("i'd ", "I'd ")
                     .Replace("i'm", "I'm")
                     .Replace("i'll", "I'll")
                     .Replace("i've", "I've");
            return str;
        }

        /// <summary>
        /// 确保逗号后有且仅有一个空格
        /// </summary>
        public static string FixCommaAfterSpace(this string str)
        {
            str = str.Replace(",", ", ").RemoveDuoSpaces();
            return str;
        }

        /// <summary>
        /// 比较两个 String 是否相等，Null、空字符串判断为相等
        /// </summary>
        public static bool IsSame(this string str1, string str2)
        {
            if (str1 == str2) return true;
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return true;
            return false;
        }

        /// <summary>
        /// 比较两个 String 是否相等，Null、空字符串、空格字符串判断为相等
        /// </summary>
        public static bool IsSameWhiteSpace(this string str1, string str2)
        {
            if (str1 == str2) return true;
            if (string.IsNullOrWhiteSpace(str1) && string.IsNullOrWhiteSpace(str2)) return true;
            return false;
        }


        /// <summary>
        /// 比较两个 String 在 Trim 后是否相等
        /// </summary>
        public static bool IsSameTrim(this string str1, string str2)
        {
            str1 = (str1 ?? string.Empty).Trim();
            str2 = (str2 ?? string.Empty).Trim();
            if (str1 == str2) return true;
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return true;
            return false;
        }

        /// <summary>
        /// 移除多个换行符
        /// </summary>
        public static string RemoveDuoBackslashN(this string str)
        {
            if (str == null) return null;
            while (str.Contains("\n\n"))
            {
                str = str.Replace("\n\n", "\n");
            }
            return str;
        }

        /// <summary>
        /// 移除 \r
        /// </summary>
        public static string RemoveBackslashR(this string str)
        {
            if (str == null) return null;
            str = str.Replace("\r", "");
            return str;
        }

        /// <summary>
        /// 是否含有 CJK 字符
        /// </summary>
        public static bool HasCJK(this string str, bool includeColon = false)
        {
            if (includeColon && str.Contains('：')) return true;
            var symbolRegex = new Regex("[`~!@#$%^&*()+=|{}':;',\\[\\].<>/?~！@#￥%……&*（）——+|{}《》【】‘；：\"”“’。，、？]");
            str = symbolRegex.Replace(str, "");
            var cjkRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}|^[\u0800-\u4e00]|^[\u4e00-\u9fa5]|^[\uac00-\ud7ff]");
            var k = cjkRegex.Matches(str);
            return k.Count > 0;
        }

        /// <summary>
        /// 是否是 CJK 字符串
        /// </summary>
        public static bool IsCJK(this string str, bool includeColon = false)
        {
            if (includeColon && str == "：") return true;
            var cjkRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            return cjkRegex.IsMatch(str);
        }

        /// <summary>
        /// 中西文间加空格 (忽略标点)
        /// </summary>
        public static string OptimizeCJK(this string str)
        {
            StringBuilder text = new();

            var cjkRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            var symbolRegex = new Regex("[`~!@#$%^&*()+=|{}':;',\\[\\].<>/?~！@#￥%……&*（）——+|{}《》【】‘；：\"”“’。，、？]");

            bool isCJKPrev = false;
            bool isSymbolPrev = false;
            foreach (char ch in str)
            {
                bool isSymbol = symbolRegex.IsMatch(ch.ToString());
                bool isCJK = cjkRegex.IsMatch(ch.ToString());

                // Use spaces to separate non-CJK words.
                if (isCJK != isCJKPrev && !isSymbol && !isSymbolPrev)
                {
                    _ = text.Append(' ');
                }

                _ = text.Append(ch);
                isCJKPrev = isCJK;
                isSymbolPrev = isSymbol;
            }
            return text.ToString().Replace("  ", " ").Trim();
        }

        /// <summary>
        /// 判断字符串是否为单个汉字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch(str, "^[\\u4e00-\\u9fa5]$");
        }

        /// <summary>
        /// 获取夹在两个字符串中间的字符串
        /// </summary>
        public static string Between(this string str, string start, string end)
        {
            if (str.IndexOf(start) != -1)
            {
                str = str[(str.IndexOf(start) + start.Length)..];
                int _end = str.IndexOf(end);
                if (_end != -1)
                {
                    str = str[.._end];
                }
                return str;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 判断是否可以作为行首
        /// </summary>
        public static bool CanStartNewLine(this string str)
        {
            if (str.EndsWith(' ') || str.EndsWith(',') || str.EndsWith('/'))
                return true;
            return false;
        }

        /// <summary>
        /// 字符串是否包含列表中的字符串
        /// </summary>
        public static bool Contains(this string str, List<string> list)
        {
            foreach (var item in list)
            {
                if (str.Contains(item)) return true;
            }
            return false;
        }

        /// <summary>
        /// 计算文本相似度函数 (适用于短文本)
        /// </summary>
        /// <param name="isCase">是否区分大小写</param>
        /// <returns>0-100</returns>
        public static double ComputeTextSame(string textX, string textY, bool isCase = false)
        {
            if (textX.Length <= 0 || textY.Length <= 0)
            {
                return (0);
            }
            if (!isCase)
            {
                textX = textX.ToLower();
                textY = textY.ToLower();
            }
            int[,] dp = new int[Math.Max(textX.Length, textY.Length) + 1, Math.Max(textX.Length, textY.Length) + 1];
            for (int x = 0; x < textX.Length; x++)
            {
                for (int y = 0; y < textY.Length; y++)
                {
                    if (textX[x] == textY[y])
                    {
                        dp[x + 1, y + 1] = dp[x, y] + 1;
                    }
                    else
                    {
                        dp[x + 1, y + 1] = Math.Max(dp[x, y + 1], dp[x + 1, y]);
                    }
                }
            }
            return Math.Round((double)dp[textX.Length, textY.Length] / Math.Max(textX.Length, textY.Length) * 100, 2);
        }

        /// <summary>
        /// 是否是繁体中文文本的信心
        /// </summary>
        /// <returns>信心百分比</returns>
        public static double TraditionalChineseConfidence(string text)
        {
            int n = 0, total = 0;
            string sc = text.ToSC();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].ToString().IsChinese())
                {
                    total++;
                    if (text[i] != sc[i])
                    {
                        n++;
                    }
                }
            }
            return (double)n / total;
        }

        /// <summary>
        /// 字符串首字母大写
        /// </summary>
        public static unsafe string ToUpperFirst(this string str)
        {
            if (str == null) return null;

            string ret = string.Copy(str);
            fixed (char* ptr = ret)
                *ptr = char.ToUpper(*ptr);
            return ret;
        }

        /// <summary>
        /// 字符串反序
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(this string str)
        {
            if (str == null) return null;

            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            string reverse = new(arr);
            return reverse;
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(this string str)
        {
            return Regex.IsMatch(str, "^\\d+$", RegexOptions.Compiled);
        }

        /// <summary>
        /// 在字符串中移除
        /// </summary>
        /// <param name="str"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
        public static string Remove(this string str, string substring)
        {
            if (str == null) return null;

            return str.Replace(substring, "");
        }

        /// <summary>
        /// 移除字符串中的控制字符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="excludeChars">不移除的控制字符</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string RemoveControlChars(this string value, params char[] excludeChars)
        {
            char[] excludeChars2 = excludeChars;
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (excludeChars2 == null)
            {
                excludeChars2 = Array.Empty<char>();
            }

            return new string(value.Where((char c) => !char.IsControl(c) || excludeChars2.Contains(c)).ToArray());
        }
    }
}
