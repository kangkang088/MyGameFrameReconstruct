using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class EventInfoBase {
}
public class EventInfo<T> : EventInfoBase {
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action) {
        actions += action;
    }
}
public class EventInfo:EventInfoBase {
    public UnityAction actions;
    public EventInfo(UnityAction action) {
        actions += action;
    }
}
/// <summary>
/// 事件中心模块
/// </summary>
public class EventCenter : BaseManager<EventCenter> {
    //用于记录 对应事件 关联的 对应逻辑
    private Dictionary<E_EventType,EventInfoBase> eventDic = new Dictionary<E_EventType,EventInfoBase>();
    private EventCenter() {

    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void EventTrigger<T>(E_EventType eventName,T info) {
        //存在关心这个事件的模块 通知执行
        if(eventDic.ContainsKey(eventName)) {
            //执行
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }
    /// <summary>
    /// 触发事件，无参数
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName) {
        //存在关心这个事件的模块 通知执行
        if(eventDic.ContainsKey(eventName)) {
            //执行
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }
    /// <summary>
    /// 添加事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    public void AddEventListener<T>(E_EventType eventName,UnityAction<T> action) {
        //如果已经存在了关心这个事件的模块，直接添加处理函数
        if(eventDic.ContainsKey(eventName)) {
            (eventDic[eventName] as EventInfo<T>).actions += action;
        }
        else//没有就创建关心者和处理逻辑
            {
            eventDic.Add(eventName,new EventInfo<T>(action));
        }
    }
    public void AddEventListener(E_EventType eventName,UnityAction action) {
        //如果已经存在了关心这个事件的模块，直接添加处理函数
        if(eventDic.ContainsKey(eventName)) {
            (eventDic[eventName] as EventInfo).actions += action;
        }
        else//没有就创建关心者和处理逻辑
            {
            eventDic.Add(eventName,new EventInfo(action));
        }
    }
    /// <summary>
    /// 移除事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    public void RemoveEventListener<T>(E_EventType eventName,UnityAction<T> action) {
        if(eventDic.ContainsKey(eventName)) {
            (eventDic[eventName] as EventInfo<T>).actions -= action;
        }
    }
    public void RemoveEventListener(E_EventType eventName,UnityAction action) {
        if(eventDic.ContainsKey(eventName)) {
            (eventDic[eventName] as EventInfo).actions -= action;
        }
    }
    /// <summary>
    /// 清除所有事件的所有监听者
    /// </summary>
    public void Clear() {
        eventDic.Clear();
    }
    /// <summary>
    /// 清除某一个事件的所有监听者
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventType eventName) {
        if(eventDic.ContainsKey(eventName)) {
            eventDic.Remove(eventName);
        }
    }
}
