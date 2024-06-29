using System;
using System.Text;
using UnityEngine.Events;

/// <summary>
/// 文本工具模块
/// </summary>
public class TextUtil
{
    #region 字符串拆分相关

    /// <summary>
    /// 拆分字符串，返回字符串数组
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="type">拆分类型 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <returns>字符串数组</returns>
    public static string[] SplitStr(string str,int type = 1)
    {
        if(str == "")
            return new string[0];
        string newStr = str;
        if(type == 1)
        {
            while(newStr.IndexOf("；") != -1)
            {
                newStr = newStr.Replace("；",";");
            }
            return newStr.Split(";");
        }
        else if(type == 2)
        {
            while(newStr.IndexOf("，") != -1)
            {
                newStr = newStr.Replace("，",",");
            }
            return newStr.Split(",");
        }
        else if(type == 3)
        {
            return newStr.Split("%");
        }
        else if(type == 4)
        {
            while(newStr.IndexOf("：") != -1)
            {
                newStr = newStr.Replace("：",":");
            }
            return newStr.Split(":");
        }
        else if(type == 5)
        {
            return newStr.Split(" ");
        }
        else if(type == 6)
        {
            return newStr.Split("|");
        }
        else if(type == 7)
        {
            return newStr.Split("_");
        }
        return new string[0];
    }

    /// <summary>
    /// 拆分字符串，返回整型数组
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="type">拆分类型 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <returns>整型数组</returns>
    public static int[] SplitStrToIntArray(string str,int type = 1)
    {
        string[] strs = SplitStr(str,type);
        if(strs.Length == 0)
            return new int[0];
        //把字符串数据转换为int数组
        return Array.ConvertAll<string,int>(strs,(str) =>
        {
            return int.Parse(str);
        });
    }

    /// <summary>
    /// 专门用来处理拆分道具组信息的方法(int)
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">道具间的分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="typeTwo">每个道具ID和数量的分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="callback">拆分结束后的回调函数</param>
    public static void SpliteStrToIntArrayTwice(string str,int typeOne,int typeTwo,UnityAction<int,int> callback)
    {
        string[] strs = SplitStr(str,typeOne);

        if(strs.Length == 0)
            return;

        int[] ints;

        for(int i = 0;i < strs.Length;i++)
        {
            ints = SplitStrToIntArray(strs[i],typeTwo);

            if(ints.Length == 0)
                continue;

            callback?.Invoke(ints[0],ints[1]);
        }
    }

    /// <summary>
    /// 专门用来处理拆分道具组信息的方法(str)
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">道具间的分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="typeTwo">每个道具ID和数量的分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="callback">拆分结束后的回调函数</param>
    public static void SpliteStrToStrArrayTwice(string str,int typeOne,int typeTwo,UnityAction<string,string> callback)
    {
        string[] strs = SplitStr(str,typeOne);

        if(strs.Length == 0)
            return;

        string[] strs2;

        for(int i = 0;i < strs.Length;i++)
        {
            strs2 = SplitStr(strs[i],typeTwo);

            if(strs2.Length == 0)
                continue;

            callback?.Invoke(strs2[0],strs2[1]);
        }
    }

    #endregion 字符串拆分相关

    #region 数字转字符串相关

    /// <summary>
    /// 得到指定长度的数值转字符串内容
    /// </summary>
    /// <param name="value">数值</param>
    /// <param name="len">长度。value长度不够，前面补0；value长度超出，保留value</param>
    /// <returns></returns>
    public static string GetStrOfNumber(int value,int len)
    {
        //ToString中传入 "Dn",代表想要将value转换为长度为n的字符串，长度不够，数字前面补0
        //如果value本身就超过了n，会保留value本身，不会裁剪
        return value.ToString($"D{len}");
    }

    /// <summary>
    /// 浮点数转换为字符串并保留小数点后n位
    /// </summary>
    /// <param name="value">浮点数</param>
    /// <param name="len">小数点后的长度</param>
    /// <returns></returns>
    public static string GetStrOfNumberWithDecimal(float value,int len)
    {
        //ToString中传入 "Fn",代表想要保留value小数点后n位，value小数点的位数大于n，n位后面的数字不会被保留，反之，补0
        return value.ToString($"F{len}");
    }

    /// <summary>
    /// 将大数据转换为指定格式的字符串
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetStrOfTheBigDataAndNumber(int num)
    {
        //大于一亿，显示n亿n千万
        if(num >= 100000000)
        {
            return BigDataAndNumberChange(num,100000000,"亿","千万");
        }
        //大于一万，显示n万n千
        else if(num >= 10000)
        {
            return BigDataAndNumberChange(num,10000,"万","千");
        }
        //显示原数字
        else
            return num.ToString();
    }

    /// <summary>
    /// 转换大数据
    /// </summary>
    /// <param name="num">大数值</param>
    /// <param name="company">数值分割基数</param>
    /// <param name="bigCompany">大单位</param>
    /// <param name="littleCompany">小单位</param>
    /// <returns></returns>
    private static string BigDataAndNumberChange(int num,int company,string bigCompany,string littleCompany)
    {
        resultStr.Clear();
        resultStr.Append(num / company);
        resultStr.Append(bigCompany);
        int tempNumber = num % company;
        tempNumber /= (company / 10);
        if(tempNumber != 0)
        {
            resultStr.Append(tempNumber);
            resultStr.Append(littleCompany);
        }
        return resultStr.ToString();
    }

    #endregion 数字转字符串相关

    #region 时间转换相关

    private static StringBuilder resultStr = new();

    /// <summary>
    /// 秒转时分秒
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="ignoreZero">是否忽略0</param>
    /// <param name="isKeepLen">是否至少保留两位</param>
    /// <param name="hourStr">小时的拼接字符</param>
    /// <param name="minuteStr">分钟拼接字符</param>
    /// <param name="secondStr">秒数拼接字符</param>
    /// <returns></returns>
    public static string SecondToHMS(int s,bool ignoreZero = false,bool isKeepLen = false,string hourStr = "时",string minuteStr = "分",string secondStr = "秒")
    {
        if(s < 0)
            s = 0;

        //计算小时
        int hour = s / 3600;
        //计算分钟
        int second = s % 3600;
        int minute = second / 60;
        //计算秒
        second = s % 60;

        resultStr.Clear();
        if(hour != 0 || !ignoreZero)
        {
            resultStr.Append(isKeepLen ? GetStrOfNumber(hour,2) : hour);
            resultStr.Append(hourStr);
        }
        if(minute != 0 || !ignoreZero || hour != 0)
        {
            resultStr.Append(isKeepLen ? GetStrOfNumber(minute,2) : minute);
            resultStr.Append(minuteStr);
        }
        if(second != 0 || !ignoreZero || hour != 0 || minute != 0)
        {
            resultStr.Append(isKeepLen ? GetStrOfNumber(second,2) : second);
            resultStr.Append(secondStr);
        }

        if(resultStr.Length == 0)
        {
            resultStr.Append(0);
            resultStr.Append(secondStr);
        }

        return resultStr.ToString();
    }

    /// <summary>
    /// 秒转00：00：00
    /// </summary>
    /// <param name="s"></param>
    /// <param name="ignoreZero"></param>
    /// <returns></returns>
    public static string SecontToHMSNormal(int s,bool ignoreZero = false)
    {
        return SecondToHMS(s,ignoreZero,true,":",":","");
    }

    #endregion 时间转换相关
}