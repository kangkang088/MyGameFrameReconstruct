using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ʱ�������࣬�洢��ʱ���������
/// </summary>
public class TimerItem : IPoolObject
{
    //��ʱ��ΨһID
    public int keyID;

    //��ʱ������ִ�еĻص�
    public UnityAction overCallback;

    //���һ��ʱ��ִ�еĻص�
    public UnityAction callback;

    //��ʱ��ʱ��(ms)
    public int allTime;

    //��¼һ��ʼ��ʱʱ����ʱ�䣬����ʱ������
    public int maxAllTime;

    //���ִ�лص���ʱ��(ms)
    public int intervalTime;

    //��¼һ��ʼ�ļ��ʱ�䣬��������
    public int maxIntervalTime;

    //�Ƿ�����ʱ��
    public bool isRunning;

    /// <summary>
    /// ��ʼ����ʱ������
    /// </summary>
    /// <param name="keyID">ΨһID</param>
    /// <param name="allTime">��ʱ��</param>
    /// <param name="overCallback">��ʱ���ʱ������Ļص�</param>
    /// <param name="intervalTime">���ִ�е�ʱ��</param>
    /// <param name="callback">���ִ��ʱ�������Ļص�</param>
    public void InitInfo(int keyID,int allTime,UnityAction overCallback,int intervalTime = 0,UnityAction callback = null)
    {
        this.keyID = keyID;
        maxAllTime = this.allTime = allTime;
        this.overCallback = overCallback;
        maxIntervalTime = this.intervalTime = intervalTime;
        this.callback = callback;
        isRunning = true;
    }

    /// <summary>
    /// ���ü�ʱ��
    /// </summary>
    public void ResetTimer()
    {
        allTime = maxAllTime;
        intervalTime = maxIntervalTime;
        isRunning = true;
    }

    /// <summary>
    /// ����ػ���ʱ����������������
    /// </summary>
    public void ResetInfo()
    {
        overCallback = null;
        callback = null;
    }
}
