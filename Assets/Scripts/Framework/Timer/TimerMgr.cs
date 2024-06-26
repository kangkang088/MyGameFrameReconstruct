using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ʱ������������Ҫ���ڿ�����ֹͣ�����õȲ����������ʱ��
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    /// <summary>
    /// ���ڼ�¼��ǰ��Ҫ������ΨһID��
    /// </summary>
    private int TIMER_KEY = 0;

    /// <summary>
    /// ���ڴ洢�͹������м�ʱ��������
    /// </summary>
    private Dictionary<int,TimerItem> timerDic = new();

    /// <summary>
    /// ���ڴ洢�͹������м�ʱ��������������Time.timeScaleӰ�죩
    /// </summary>
    private Dictionary<int,TimerItem> realTimerDic = new();

    //Э�̺������ض���
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);

    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);

    /// <summary>
    /// ���Ƴ��ļ�ʱ���б�
    /// </summary>
    private List<TimerItem> deleteList = new();

    //��ʱ����������ʱ��Э�̶���
    private Coroutine timer;

    private Coroutine realTimer;

    //��ʱ����������Ψһ�ļ�ʱ�õ�Эͬ����ļ��ʱ��
    private const float intervalTime = 0.1f;

    private TimerMgr()
    {
        Start();
    }

    /// <summary>
    /// ������ʱ���������ķ���
    /// </summary>
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false,timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true,realTimerDic));
    }

    /// <summary>
    /// �رռ�ʱ���������ķ���
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
            //100ms����һ�μ�ʱ
            if(isRealTimer)
                yield return waitForSecondsRealtime;
            else
                yield return waitForSeconds;

            //�������м�ʱ������������
            foreach(TimerItem item in timerDic.Values)
            {
                if(!item.isRunning)
                    continue;

                //�жϼ�ʱ���Ƿ��м��ʱ��ִ�е�����
                if(item.callback != null)
                {
                    item.intervalTime -= (int)(intervalTime * 1000);
                    //����һ�μ��ʱ��ִ��
                    if(item.intervalTime <= 0)
                    {
                        //ִ�м���ص�
                        item.callback.Invoke();
                        //���ü��ʱ��
                        item.intervalTime = item.maxIntervalTime;
                    }
                }

                //��ʱ�����
                item.allTime -= (int)(intervalTime * 1000);
                //��ʱ����
                if(item.allTime <= 0)
                {
                    item.overCallback.Invoke();
                    deleteList.Add(item);
                }
            }

            //�Ƴ����Ƴ��б��еļ�����
            for(int i = 0;i < deleteList.Count;i++)
            {
                timerDic.Remove(deleteList[i].keyID);
                //���뻺�����
                PoolMgr.Instance.PushObj(deleteList[i]);
            }
            deleteList.Clear();
        }
    }

    /// <summary>
    /// ����������ʱ��
    /// </summary>
    /// /// <param name="isRealTimer">�ܼ�ʱʱ��(ms)</param>
    /// <param name="allTime">�ܼ�ʱʱ��(ms)</param>
    /// <param name="overCallback">��ʱ��ص�</param>
    /// <param name="intervalTime">���ʱ��</param>
    /// <param name="callback">���ʱ��ص�</param>
    /// <returns>��ʱ��ΨһID</returns>
    public int CreateTimer(bool isRealTimer,int allTime,UnityAction overCallback,int intervalTime = 0,UnityAction callback = null)
    {
        //����ΨһID
        int keyID = ++TIMER_KEY;
        TimerItem timerItem = PoolMgr.Instance.GetObj<TimerItem>();
        //��ʼ������
        timerItem.InitInfo(keyID,allTime,overCallback,intervalTime,callback);
        //ѹ���ֵ�
        if(isRealTimer)
            realTimerDic.Add(keyID,timerItem);
        else
            timerDic.Add(keyID,timerItem);
        return keyID;
    }

    /// <summary>
    /// �Ƴ�������ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
    public void RemoveTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
        {
            //���뻺���
            PoolMgr.Instance.PushObj(timerDic[keyID]);
            timerDic.Remove(keyID);
        }
        else if(realTimerDic.ContainsKey(keyID))
        {
            //���뻺���
            PoolMgr.Instance.PushObj(realTimerDic[keyID]);
            realTimerDic.Remove(keyID);
        }
    }

    /// <summary>
    /// ���õ�����ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
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
    /// ����������ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
    public void StartTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
            timerDic[keyID].isRunning = true;
        else if(realTimerDic.ContainsKey(keyID))
            realTimerDic[keyID].isRunning = true;
    }

    /// <summary>
    /// ֹͣ������ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
    public void StopTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
            timerDic[keyID].isRunning = false;
        else if(realTimerDic.ContainsKey(keyID))
            realTimerDic[keyID].isRunning = false;
    }
}