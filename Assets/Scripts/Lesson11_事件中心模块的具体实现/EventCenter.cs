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
/// �¼�����ģ��
/// </summary>
public class EventCenter : BaseManager<EventCenter> {
    //���ڼ�¼ ��Ӧ�¼� ������ ��Ӧ�߼�
    private Dictionary<E_EventType,EventInfoBase> eventDic = new Dictionary<E_EventType,EventInfoBase>();
    private EventCenter() {

    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="eventName">�¼���</param>
    public void EventTrigger<T>(E_EventType eventName,T info) {
        //���ڹ�������¼���ģ�� ִ֪ͨ��
        if(eventDic.ContainsKey(eventName)) {
            //ִ��
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }
    /// <summary>
    /// �����¼����޲���
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName) {
        //���ڹ�������¼���ģ�� ִ֪ͨ��
        if(eventDic.ContainsKey(eventName)) {
            //ִ��
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }
    /// <summary>
    /// ����¼�������
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    public void AddEventListener<T>(E_EventType eventName,UnityAction<T> action) {
        //����Ѿ������˹�������¼���ģ�飬ֱ����Ӵ�����
        if(eventDic.ContainsKey(eventName)) {
            (eventDic[eventName] as EventInfo<T>).actions += action;
        }
        else//û�оʹ��������ߺʹ����߼�
            {
            eventDic.Add(eventName,new EventInfo<T>(action));
        }
    }
    public void AddEventListener(E_EventType eventName,UnityAction action) {
        //����Ѿ������˹�������¼���ģ�飬ֱ����Ӵ�����
        if(eventDic.ContainsKey(eventName)) {
            (eventDic[eventName] as EventInfo).actions += action;
        }
        else//û�оʹ��������ߺʹ����߼�
            {
            eventDic.Add(eventName,new EventInfo(action));
        }
    }
    /// <summary>
    /// �Ƴ��¼�������
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
    /// ��������¼������м�����
    /// </summary>
    public void Clear() {
        eventDic.Clear();
    }
    /// <summary>
    /// ���ĳһ���¼������м�����
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventType eventName) {
        if(eventDic.ContainsKey(eventName)) {
            eventDic.Remove(eventName);
        }
    }
}
