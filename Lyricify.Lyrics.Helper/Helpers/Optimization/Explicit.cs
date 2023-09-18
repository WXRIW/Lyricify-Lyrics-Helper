namespace Lyricify.Lyrics.Helpers.Optimization
{
    public class Explicit
    {
        /// <summary>
        /// 处理字符串中的 Explicit 内容
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="strong">是否是增强处理 (完全屏蔽为星号)</param>
        /// <returns>处理后的字符串</returns>
        public static string Clean(string str, bool strong = false)
        {
            if (strong)
            {
                str = FixExplicit(str)
                    .Replace("bitches", "*****")
                    .Replace("Bitches", "*****")
                    .Replace("bitch", "*****")
                    .Replace("Bitch", "*****")
                    .Replace("damn", "****")
                    .Replace("Damn", "****")
                    .Replace("dammit", "******")
                    .Replace("Dammit", "******")
                    .Replace("dick", "****")
                    .Replace("Dick", "****")
                    .Replace("dope", "****")
                    .Replace("Dope", "****")
                    .Replace("fuck", "****")
                    .Replace("Fuck", "****")
                    .Replace("nigga", "*****")
                    .Replace("Nigga", "*****")
                    .Replace("nigras", "******")
                    .Replace("Nigras", "******")
                    .Replace("pussy", "*****")
                    .Replace("Pussy", "*****")
                    .Replace("sex", "***")
                    .Replace("Sex", "***")
                    .Replace("shit", "****")
                    .Replace("Shit", "****")
                    .Replace("weed", "****")
                    .Replace("Weed", "****")
                    .Replace("whore", "*****")
                    .Replace("Whore", "*****")
                    .Replace("cocaine", "*******")
                    .Replace("Cocaine", "*******")
                    .Replace("drug", "****")
                    .Replace("Drug", "****");
                // Fix ass
                for (int i = 0; i < str.Length - 2; i++)
                {
                    if (str[i..(i + 3)] == "ass")
                    {
                        if (i > 0 && str[i - 1] != ' ' && str[i - 1] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        else if (i + 3 < str.Length && str[i + 3] != ' ' && str[i + 3] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        str = str.Remove(i, 3);
                        str = str.Insert(i, "***");
                        i += 2;
                    }
                }
                // Fix Ass
                for (int i = 0; i < str.Length - 2; i++)
                {
                    if (str[i..(i + 3)] == "Ass")
                    {
                        if (i > 0 && str[i - 1] != ' ' && str[i - 1] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        else if (i + 3 < str.Length && str[i + 3] != ' ' && str[i + 3] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        str = str.Remove(i, 3);
                        str = str.Insert(i, "***");
                        i += 2;
                    }
                }
                // Fix hoe
                for (int i = 0; i < str.Length - 2; i++)
                {
                    if (str[i..(i + 3)] == "hoe")
                    {
                        if (i > 0 && str[i - 1] != ' ' && str[i - 1] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        else if (i + 3 < str.Length && str[i + 3] != ' ' && str[i + 3] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        str = str.Remove(i, 3);
                        str = str.Insert(i, "***");
                        i += 2;
                    }
                }
            }
            else
            {
                str = str
                    .Replace("bitch", "b***h")
                    .Replace("Bitch", "B***h")
                    .Replace("damn", "d**n")
                    .Replace("Damn", "D**n")
                    .Replace("dammit", "D**mit")
                    .Replace("Dammit", "D**mit")
                    .Replace("dick", "d**k")
                    .Replace("Dick", "D**k")
                    .Replace("dope", "d**e")
                    .Replace("Dope", "D**e")
                    .Replace("fuck", "f**k")
                    .Replace("Fuck", "F**k")
                    .Replace("nigga", "n***a")
                    .Replace("Nigga", "N***a")
                    .Replace("nigras", "n***as")
                    .Replace("Nigras", "N***as")
                    .Replace("pussy", "p***y")
                    .Replace("Pussy", "P***y")
                    .Replace("sex", "s*x")
                    .Replace("Sex", "S*x")
                    .Replace("shit", "s**t")
                    .Replace("Shit", "S**t")
                    .Replace("weed", "w**d")
                    .Replace("Weed", "W**d")
                    .Replace("whore", "w***e")
                    .Replace("Whore", "W***e");
                // Fix ass
                for (int i = 0; i < str.Length - 2; i++)
                {
                    if (str[i..(i + 3)] == "ass")
                    {
                        if (i > 0 && str[i - 1] != ' ' && str[i - 1] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        else if (i + 3 < str.Length && str[i + 3] != ' ' && str[i + 3] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        str = str.Remove(i, 3);
                        str = str.Insert(i, "a*s");
                        i += 2;
                    }
                }
                // Fix Ass
                for (int i = 0; i < str.Length - 2; i++)
                {
                    if (str[i..(i + 3)] == "Ass")
                    {
                        if (i > 0 && str[i - 1] != ' ' && str[i - 1] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        else if (i + 3 < str.Length && str[i + 3] != ' ' && str[i + 3] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        str = str.Remove(i, 3);
                        str = str.Insert(i, "A*s");
                        i += 2;
                    }
                }
                // Fix hoe
                for (int i = 0; i < str.Length - 2; i++)
                {
                    if (str[i..(i + 3)] == "hoe")
                    {
                        if (i > 0 && str[i - 1] != ' ' && str[i - 1] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        else if (i + 3 < str.Length && str[i + 3] != ' ' && str[i + 3] != '-')
                        {
                            i += 2;
                            continue;
                        }
                        str = str.Remove(i, 3);
                        str = str.Insert(i, "h*e");
                        i += 2;
                    }
                }
            }
            return str;
        }

        /// <summary>
        /// 修复字符串，恢复到 Explicit 状态
        /// </summary>
        /// <param name="str">要修复的字符串</param>
        /// <returns>修护后的字符串</returns>
        public static string FixExplicit(string str)
        {
            return str
                .Replace("a*s", "ass")
                .Replace("a**", "ass")
                .Replace("b***h", "bitch")
                .Replace("B***h", "Bitch")
                .Replace("b**ch", "bitch")
                .Replace("B**ch", "Bitch")
                .Replace("b*****s", "bitches")
                .Replace("B*****s", "Bitches")
                .Replace("d**n", "damn")
                .Replace("D**n", "Damn")
                .Replace("d**mit", "dammit")
                .Replace("D**mit", "Dammit")
                .Replace("d**k", "dick")
                .Replace("D**k", "Dick")
                .Replace("d**e", "dope")
                .Replace("D**e", "Dope")
                .Replace("f**k", "fuck")
                .Replace("F**k", "Fuck")
                .Replace("fu*k", "fuck")
                .Replace("Fu*k", "Fuck")
                .Replace("h*e", "hoe")
                .Replace("H*e", "Hoe")
                .Replace("h**s", "hoes")
                .Replace("H**s", "Hoes")
                .Replace("mother****", "motherfuck")
                .Replace("Mother****", "Motherfuck")
                .Replace("n***a", "nigga")
                .Replace("N***a", "Nigga")
                .Replace("n**ga", "nigga")
                .Replace("N**ga", "Nigga")
                .Replace("ni**a", "nigga")
                .Replace("Ni**a", "Nigga")
                .Replace("n***as", "nigras")
                .Replace("N***as", "Nigras")
                .Replace("p***y", "pussy")
                .Replace("P***y", "Pussy")
                .Replace("p**sy", "pussy")
                .Replace("P**sy", "Pussy")
                .Replace("sh*t", "shit")
                .Replace("Sh*t", "Shit")
                .Replace("s*x", "sex")
                .Replace("S*x", "Sex")
                .Replace("s**t", "shit")
                .Replace("S**t", "shit")
                .Replace("w**d", "weed")
                .Replace("W**d", "Weed")
                .Replace("w***e", "whore")
                .Replace("W***e", "Whore")
                .Replace("c*****e", "cocaine")
                .Replace("C*****e", "Cocaine")
                .Replace("d**g", "drug")
                .Replace("D**g", "Drug");
        }
    }
}
