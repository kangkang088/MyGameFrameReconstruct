using System;
using System.Text;
using UnityEngine.Events;

/// <summary>
/// �ı�����ģ��
/// </summary>
public class TextUtil
{
    #region �ַ���������

    /// <summary>
    /// ����ַ����������ַ�������
    /// </summary>
    /// <param name="str">����ֵ��ַ���</param>
    /// <param name="type">������� 1-; 2-, 3-% 4-: 5-�ո� 6-| 7-_</param>
    /// <returns>�ַ�������</returns>
    public static string[] SplitStr(string str,int type = 1)
    {
        if(str == "")
            return new string[0];
        string newStr = str;
        if(type == 1)
        {
            while(newStr.IndexOf("��") != -1)
            {
                newStr = newStr.Replace("��",";");
            }
            return newStr.Split(";");
        }
        else if(type == 2)
        {
            while(newStr.IndexOf("��") != -1)
            {
                newStr = newStr.Replace("��",",");
            }
            return newStr.Split(",");
        }
        else if(type == 3)
        {
            return newStr.Split("%");
        }
        else if(type == 4)
        {
            while(newStr.IndexOf("��") != -1)
            {
                newStr = newStr.Replace("��",":");
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
    /// ����ַ�����������������
    /// </summary>
    /// <param name="str">����ֵ��ַ���</param>
    /// <param name="type">������� 1-; 2-, 3-% 4-: 5-�ո� 6-| 7-_</param>
    /// <returns>��������</returns>
    public static int[] SplitStrToIntArray(string str,int type = 1)
    {
        string[] strs = SplitStr(str,type);
        if(strs.Length == 0)
            return new int[0];
        //���ַ�������ת��Ϊint����
        return Array.ConvertAll<string,int>(strs,(str) =>
        {
            return int.Parse(str);
        });
    }

    /// <summary>
    /// ר�����������ֵ�������Ϣ�ķ���(int)
    /// </summary>
    /// <param name="str">����ֵ��ַ���</param>
    /// <param name="typeOne">���߼�ķָ��� 1-; 2-, 3-% 4-: 5-�ո� 6-| 7-_</param>
    /// <param name="typeTwo">ÿ������ID�������ķָ��� 1-; 2-, 3-% 4-: 5-�ո� 6-| 7-_</param>
    /// <param name="callback">��ֽ�����Ļص�����</param>
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
    /// ר�����������ֵ�������Ϣ�ķ���(str)
    /// </summary>
    /// <param name="str">����ֵ��ַ���</param>
    /// <param name="typeOne">���߼�ķָ��� 1-; 2-, 3-% 4-: 5-�ո� 6-| 7-_</param>
    /// <param name="typeTwo">ÿ������ID�������ķָ��� 1-; 2-, 3-% 4-: 5-�ո� 6-| 7-_</param>
    /// <param name="callback">��ֽ�����Ļص�����</param>
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

    #endregion �ַ���������

    #region ����ת�ַ������

    /// <summary>
    /// �õ�ָ�����ȵ���ֵת�ַ�������
    /// </summary>
    /// <param name="value">��ֵ</param>
    /// <param name="len">���ȡ�value���Ȳ�����ǰ�油0��value���ȳ���������value</param>
    /// <returns></returns>
    public static string GetStrOfNumber(int value,int len)
    {
        //ToString�д��� "Dn",������Ҫ��valueת��Ϊ����Ϊn���ַ��������Ȳ���������ǰ�油0
        //���value����ͳ�����n���ᱣ��value��������ü�
        return value.ToString($"D{len}");
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���������С�����nλ
    /// </summary>
    /// <param name="value">������</param>
    /// <param name="len">С�����ĳ���</param>
    /// <returns></returns>
    public static string GetStrOfNumberWithDecimal(float value,int len)
    {
        //ToString�д��� "Fn",������Ҫ����valueС�����nλ��valueС�����λ������n��nλ��������ֲ��ᱻ��������֮����0
        return value.ToString($"F{len}");
    }

    /// <summary>
    /// ��������ת��Ϊָ����ʽ���ַ���
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetStrOfTheBigDataAndNumber(int num)
    {
        //����һ�ڣ���ʾn��nǧ��
        if(num >= 100000000)
        {
            return BigDataAndNumberChange(num,100000000,"��","ǧ��");
        }
        //����һ����ʾn��nǧ
        else if(num >= 10000)
        {
            return BigDataAndNumberChange(num,10000,"��","ǧ");
        }
        //��ʾԭ����
        else
            return num.ToString();
    }

    /// <summary>
    /// ת��������
    /// </summary>
    /// <param name="num">����ֵ</param>
    /// <param name="company">��ֵ�ָ����</param>
    /// <param name="bigCompany">��λ</param>
    /// <param name="littleCompany">С��λ</param>
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

    #endregion ����ת�ַ������

    #region ʱ��ת�����

    private static StringBuilder resultStr = new();

    /// <summary>
    /// ��תʱ����
    /// </summary>
    /// <param name="s">����</param>
    /// <param name="ignoreZero">�Ƿ����0</param>
    /// <param name="isKeepLen">�Ƿ����ٱ�����λ</param>
    /// <param name="hourStr">Сʱ��ƴ���ַ�</param>
    /// <param name="minuteStr">����ƴ���ַ�</param>
    /// <param name="secondStr">����ƴ���ַ�</param>
    /// <returns></returns>
    public static string SecondToHMS(int s,bool ignoreZero = false,bool isKeepLen = false,string hourStr = "ʱ",string minuteStr = "��",string secondStr = "��")
    {
        if(s < 0)
            s = 0;

        //����Сʱ
        int hour = s / 3600;
        //�������
        int second = s % 3600;
        int minute = second / 60;
        //������
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
    /// ��ת00��00��00
    /// </summary>
    /// <param name="s"></param>
    /// <param name="ignoreZero"></param>
    /// <returns></returns>
    public static string SecontToHMSNormal(int s,bool ignoreZero = false)
    {
        return SecondToHMS(s,ignoreZero,true,":",":","");
    }

    #endregion ʱ��ת�����
}