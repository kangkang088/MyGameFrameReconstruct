using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器。主要用于开启，停止，重置等操作来管理计时器
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    /// <summary>
    /// 用于记录当前将要创建的唯一ID的
    /// </summary>
    private int TIMER_KEY = 0;

    /// <summary>
    /// 用于存储和管理所有计时器的容器
    /// </summary>
    private Dictionary<int,TimerItem> timerDic = new();

    /// <summary>
    /// 用于存储和管理所有计时器的容器（不受Time.timeScale影响）
    /// </summary>
    private Dictionary<int,TimerItem> realTimerDic = new();

    //协程函数返回对象
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);

    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);

    /// <summary>
    /// 待移除的计时器列表
    /// </summary>
    private List<TimerItem> deleteList = new();

    //计时器管理器计时的协程对象
    private Coroutine timer;

    private Coroutine realTimer;

    //计时器管理器中唯一的计时用的协同程序的间隔时间
    private const float intervalTime = 0.1f;

    private TimerMgr()
    {
        Start();
    }

    /// <summary>
    /// 开启计时器管理器的方法
    /// </summary>
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false,timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true,realTimerDic));
    }

    /// <summary>
    /// 关闭计时器管理器的方法
    /// </summary>
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }

    private IEnumerator StartTiming(bool isRealTimer,Dictionary<int,TimerItem> timerDic)
    {
        while(true)
        {
            //100ms进行一次计时
            if(isRealTimer)
                yield return waitForSecondsRealtime;
            else
                yield return waitForSeconds;

            //遍历所有计时器，更新数据
            foreach(TimerItem item in timerDic.Values)
            {
                if(!item.isRunning)
                    continue;

                //判断计时器是否有间隔时间执行的需求
                if(item.callback != null)
                {
                    item.intervalTime -= (int)(intervalTime * 1000);
                    //满足一次间隔时间执行
                    if(item.intervalTime <= 0)
                    {
                        //执行间隔回调
                        item.callback.Invoke();
                        //重置间隔时间
                        item.intervalTime = item.maxIntervalTime;
                    }
                }

                //总时间更新
                item.allTime -= (int)(intervalTime * 1000);
                //计时结束
                if(item.allTime <= 0)
                {
                    item.overCallback.Invoke();
                    deleteList.Add(item);
                }
            }

            //移除待移除列表中的计数器
            for(int i = 0;i < deleteList.Count;i++)
            {
                timerDic.Remove(deleteList[i].keyID);
                //放入缓存池中
                PoolMgr.Instance.PushObj(deleteList[i]);
            }
            deleteList.Clear();
        }
    }

    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// /// <param name="isRealTimer">总计时时间(ms)</param>
    /// <param name="allTime">总计时时间(ms)</param>
    /// <param name="overCallback">总时间回调</param>
    /// <param name="intervalTime">间隔时间</param>
    /// <param name="callback">间隔时间回调</param>
    /// <returns>计时器唯一ID</returns>
    public int CreateTimer(bool isRealTimer,int allTime,UnityAction overCallback,int intervalTime = 0,UnityAction callback = null)
    {
        //构建唯一ID
        int keyID = ++TIMER_KEY;
        TimerItem timerItem = PoolMgr.Instance.GetObj<TimerItem>();
        //初始化数据
        timerItem.InitInfo(keyID,allTime,overCallback,intervalTime,callback);
        //压入字典
        if(isRealTimer)
            realTimerDic.Add(keyID,timerItem);
        else
            timerDic.Add(keyID,timerItem);
        return keyID;
    }

    /// <summary>
    /// 移除单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void RemoveTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
        {
            //放入缓存池
            PoolMgr.Instance.PushObj(timerDic[keyID]);
            timerDic.Remove(keyID);
        }
        else if(realTimerDic.ContainsKey(keyID))
        {
            //放入缓存池
            PoolMgr.Instance.PushObj(realTimerDic[keyID]);
            realTimerDic.Remove(keyID);
        }
    }

    /// <summary>
    /// 重置单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void ResetTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].ResetTimer();
        }
        else if(realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].ResetTimer();
        }
    }

    /// <summary>
    /// 开启单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StartTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
            timerDic[keyID].isRunning = true;
        else if(realTimerDic.ContainsKey(keyID))
            realTimerDic[keyID].isRunning = true;
    }

    /// <summary>
    /// 停止单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StopTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
            timerDic[keyID].isRunning = false;
        else if(realTimerDic.ContainsKey(keyID))
            realTimerDic[keyID].isRunning = false;
    }
}