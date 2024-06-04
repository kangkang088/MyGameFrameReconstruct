using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ����Monoģ�������
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr> {
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;
    /// <summary>
    /// ���Update֡���¼�������
    /// </summary>
    /// <param name="updateFunction"></param>
    public void AddUpdateListener(UnityAction updateFunction) {
        updateEvent += updateFunction;
        Action action = () => { };
    }
    /// <summary>
    /// �Ƴ�Update֡���¼�������
    /// </summary>
    /// <param name="updateFunction"></param>
    public void RemoveUpdateListener(UnityAction updateFunction) {
        updateEvent -= updateFunction;
    }
    /// <summary>
    /// ���FixedUpdate��ʱ���¼�������
    /// </summary>
    /// <param name="fixedUpdateFunction"></param>
    public void AddFixedUpdateListener(UnityAction fixedUpdateFunction) {
        fixedUpdateEvent += fixedUpdateFunction;
    }
    /// <summary>
    /// �Ƴ�FixedUpdate��ʱ���¼�������
    /// </summary>
    /// <param name="fixedUpdateFunction"></param>
    public void RemoveFixedUpdateListener( UnityAction fixedUpdateFunction) {
        fixedUpdateEvent -= fixedUpdateFunction;
    }
    /// <summary>
    /// ���LateUpdate֡���¼�������
    /// </summary>
    /// <param name="lateUpdateFunction"></param>
    public void AddLateUpdateListener(UnityAction lateUpdateFunction) {
        lateUpdateEvent += lateUpdateFunction;
    }
    /// <summary>
    /// �Ƴ�LateUpdate֡���¼�������
    /// </summary>
    /// <param name="lateUpdateFunction"></param>
    public void RemoveLateUpdateListener( UnityAction lateUpdateFunction) {
        lateUpdateEvent -= lateUpdateFunction;
    }
    private void Update() {
        updateEvent?.Invoke();
    }
    private void FixedUpdate() {
        fixedUpdateEvent?.Invoke();
    }
    private void LateUpdate() {
        lateUpdateEvent?.Invoke();
    }
}
