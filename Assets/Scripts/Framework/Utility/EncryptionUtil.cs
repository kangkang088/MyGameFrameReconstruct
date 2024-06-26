using UnityEngine;

/// <summary>
/// 加密工具类，提供加密需求
/// </summary>
public class EncryptionUtil
{
    /// <summary>
    /// 获取随机密钥
    /// </summary>
    /// <returns></returns>
    public static int GetRandomKey()
    {
        return Random.Range(0,10000) + 5;
    }

    /// <summary>
    /// 加密数据
    /// </summary>
    /// <param name="value">数据</param>
    /// <param name="key">密钥</param>
    /// <returns></returns>
    public static int LockValue(int value,int key)
    {
        value ^= (key % 9);
        value ^= 0xADAD;
        value ^= (1 << 5);
        value += key;
        return value;
    }

    public static long LockValue(long value,int key)
    {
        value ^= (key % 9);
        value ^= 0xADAD;
        value ^= (1 << 5);
        value += key;
        return value;
    }

    /// <summary>
    /// 解密数据
    /// </summary>
    /// <param name="locakValue">加密数据</param>
    /// <param name="key">密钥</param>
    /// <returns></returns>
    public static int UnlockValue(int locakValue,int key)
    {
        //没有加密过，没初始化的数据，直接返回给外部
        if(locakValue == 0)
            return locakValue;

        locakValue -= key;
        locakValue ^= (key % 9);
        locakValue ^= 0xADAD;
        locakValue ^= (1 << 5);
        return locakValue;
    }

    public static long UnlockValue(long locakValue,int key)
    {
        //没有加密过，没初始化的数据，直接返回给外部
        if(locakValue == 0)
            return locakValue;

        locakValue -= key;
        locakValue ^= (key % 9);
        locakValue ^= 0xADAD;
        locakValue ^= (1 << 5);
        return locakValue;
    }
}