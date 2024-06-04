using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 公共Mono模块管理器
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr> {
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;
    /// <summary>
    /// 添加Update帧更新监听函数
    /// </summary>
    /// <param name="updateFunction"></param>
    public void AddUpdateListener(UnityAction updateFunction) {
        updateEvent += updateFunction;
        Action action = () => { };
    }
    /// <summary>
    /// 移除Update帧更新监听函数
    /// </summary>
    /// <param name="updateFunction"></param>
    public void RemoveUpdateListener(UnityAction updateFunction) {
        updateEvent -= updateFunction;
    }
    /// <summary>
    /// 添加FixedUpdate定时更新监听函数
    /// </summary>
    /// <param name="fixedUpdateFunction"></param>
    public void AddFixedUpdateListener(UnityAction fixedUpdateFunction) {
        fixedUpdateEvent += fixedUpdateFunction;
    }
    /// <summary>
    /// 移除FixedUpdate定时更新监听函数
    /// </summary>
    /// <param name="fixedUpdateFunction"></param>
    public void RemoveFixedUpdateListener( UnityAction fixedUpdateFunction) {
        fixedUpdateEvent -= fixedUpdateFunction;
    }
    /// <summary>
    /// 添加LateUpdate帧更新监听函数
    /// </summary>
    /// <param name="lateUpdateFunction"></param>
    public void AddLateUpdateListener(UnityAction lateUpdateFunction) {
        lateUpdateEvent += lateUpdateFunction;
    }
    /// <summary>
    /// 移除LateUpdate帧更新监听函数
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
