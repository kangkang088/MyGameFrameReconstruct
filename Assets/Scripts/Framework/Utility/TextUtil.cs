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

    #endregion 数字转字符串相关

    #region 时间转换相关

    private static StringBuilder resultStr = new();

    /// <summary>
    /// 秒转时分秒
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="ignoreZero">是否忽略0</param>
    /// <param name="hourStr"></param>
    /// <param name="minuteStr"></param>
    /// <param name="secondStr"></param>
    /// <returns></returns>
    public static string SecondToHMS(int s,bool ignoreZero = false,string hourStr = "时",string minuteStr = "分",string secondStr = "秒")
    {
        //计算小时
        int hour = s / 3600;
        //计算分钟
        int second = s % 3600;
        int minute = s / 60;
        //计算秒
        second = s % 60;

        resultStr.Clear();
        if(hour != 0 || !ignoreZero)
        {
            resultStr.Append(hour);
            resultStr.Append(hourStr);
        }
        if(minute != 0 || !ignoreZero || hour != 0)
        {
            resultStr.Append(minute);
            resultStr.Append(minuteStr);
        }
        if(second != 0 || !ignoreZero || hour != 0 || minute != 0)
        {
            resultStr.Append(second);
            resultStr.Append(secondStr);
        }

        if(resultStr.Length == 0)
        {
            resultStr.Append(0);
            resultStr.Append(secondStr);
        }

        return resultStr.ToString();
    }

    #endregion 时间转换相关
}