using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载式继承Mono的单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;
    public static T Instance {
        get {
            return instance;
        }
    }
    protected virtual void Awake() {
        //已经存在单例对象，不需要再创建了
        if(instance != null) {
            Destroy(this);
            return;
        }
        instance = this as T;
        //挂载继承该单例模式基类的脚本的对象过场景不移除，保证游戏整个生命周期中都存在
        DontDestroyOnLoad(this.gameObject);
    }
}
