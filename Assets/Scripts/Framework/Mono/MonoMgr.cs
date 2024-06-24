using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ����Monoģ�������
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    private event UnityAction fixedUpdateEvent;
    private event UnityAction updateEvent;
    private event UnityAction lateUpdateEvent;

    public void AddFixedUpdateListener(UnityAction action)
    {
        fixedUpdateEvent += action;
    }

    public void AddUpdateListener(UnityAction action)
    {
        updateEvent += action;
    }

    public void AddLateUpdateListener(UnityAction action)
    {
        lateUpdateEvent += action;
    }
    public void RemoveFixedUpdateListener(UnityAction action)
    {
        fixedUpdateEvent -= action;
    }

    public void RemoveUpdateListener(UnityAction action)
    {
        updateEvent -= action;
    }

    public void RemoveLateUpdateListener(UnityAction action)
    {
        lateUpdateEvent -= action;
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }
}