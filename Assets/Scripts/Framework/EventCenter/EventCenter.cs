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
/// 事件中心模块
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    private EventCenter()
    {
    }

    //记录事件关联的逻辑
    private Dictionary<E_EventType,EventInfoBase> eventDic = new Dictionary<E_EventType,EventInfoBase>();

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名</param>
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
    /// 添加事件的监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">要添加的函数</param>
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
    /// 移除事件监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">要移除的函数</param>
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
    /// 清空指定事件的监听
    /// </summary>
    /// <param name="eventName">指定事件的名字</param>
    public void Clear(E_EventType eventName)
    {
        if(eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }

    /// <summary>
    /// 清空所有事件的监听
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}