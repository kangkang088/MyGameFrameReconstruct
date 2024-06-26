using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器对象类，存储计时器相关数据
/// </summary>
public class TimerItem : IPoolObject
{
    //计时器唯一ID
    public int keyID;

    //计时结束后执行的回调
    public UnityAction overCallback;

    //间隔一定时间执行的回调
    public UnityAction callback;

    //计时总时间(ms)
    public int allTime;

    //记录一开始计时时的总时间，用于时间重置
    public int maxAllTime;

    //间隔执行回调的时间(ms)
    public int intervalTime;

    //记录一开始的间隔时间，用于重置
    public int maxIntervalTime;

    //是否开启计时器
    public bool isRunning;

    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一ID</param>
    /// <param name="allTime">总时间</param>
    /// <param name="overCallback">总时间计时结束后的回调</param>
    /// <param name="intervalTime">间隔执行的时间</param>
    /// <param name="callback">间隔执行时间结束后的回调</param>
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
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        allTime = maxAllTime;
        intervalTime = maxIntervalTime;
        isRunning = true;
    }

    /// <summary>
    /// 缓存池回收时，清空相关引用数据
    /// </summary>
    public void ResetInfo()
    {
        overCallback = null;
        callback = null;
    }
}
