using UnityEngine;

/// <summary>
/// ���ܹ����࣬�ṩ��������
/// </summary>
public class EncryptionUtil
{
    /// <summary>
    /// ��ȡ�����Կ
    /// </summary>
    /// <returns></returns>
    public static int GetRandomKey()
    {
        return Random.Range(0,10000) + 5;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="value">����</param>
    /// <param name="key">��Կ</param>
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
    /// ��������
    /// </summary>
    /// <param name="locakValue">��������</param>
    /// <param name="key">��Կ</param>
    /// <returns></returns>
    public static int UnlockValue(int locakValue,int key)
    {
        //û�м��ܹ���û��ʼ�������ݣ�ֱ�ӷ��ظ��ⲿ
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
        //û�м��ܹ���û��ʼ�������ݣ�ֱ�ӷ��ظ��ⲿ
        if(locakValue == 0)
            return locakValue;

        locakValue -= key;
        locakValue ^= (key % 9);
        locakValue ^= 0xADAD;
        locakValue ^= (1 << 5);
        return locakValue;
    }
}