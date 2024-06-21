using System.Collections.Generic;
using UnityEngine.Events;

public abstract class EventInfoBase
{
    
}

public class EventInfo<T> : EventInfoBase
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> actions)
    {
        this.actions = actions;
    }
}

public class EventInfo : EventInfoBase
{
    public UnityAction actions;

    public EventInfo(UnityAction actions)
    {
        this.actions = actions;
    }
}

/// <summary>
/// �¼�����ģ��
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    private EventCenter()
    {
    }

    //��¼�¼��������߼�
    private Dictionary<E_EventType,EventInfoBase> eventDic = new Dictionary<E_EventType,EventInfoBase>();

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="eventName">�¼���</param>
    public void EventTrigger<T>(E_EventType eventName,T info)
    {
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    public void EventTrigger(E_EventType eventName)
    {
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }

    /// <summary>
    /// ����¼��ļ�����
    /// </summary>
    /// <param name="eventName">�¼���</param>
    /// <param name="action">Ҫ��ӵĺ���</param>
    public void AddListener<T>(E_EventType eventName,UnityAction<T> action)
    {
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(eventName,new EventInfo<T>(action));
        }
    }
    public void AddListener(E_EventType eventName,UnityAction action)
    {
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(eventName,new EventInfo(action));
        }
    }

    /// <summary>
    /// �Ƴ��¼�������
    /// </summary>
    /// <param name="eventName">�¼���</param>
    /// <param name="action">Ҫ�Ƴ��ĺ���</param>
    public void RemoveListener<T>(E_EventType eventName,UnityAction<T> action)
    {
        if(eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions -= action;
    }
    public void RemoveListener(E_EventType eventName,UnityAction action)
    {
        if(eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo).actions -= action;
    }

    /// <summary>
    /// ���ָ���¼��ļ���
    /// </summary>
    /// <param name="eventName">ָ���¼�������</param>
    public void Clear(E_EventType eventName)
    {
        if(eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }

    /// <summary>
    /// ��������¼��ļ���
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}