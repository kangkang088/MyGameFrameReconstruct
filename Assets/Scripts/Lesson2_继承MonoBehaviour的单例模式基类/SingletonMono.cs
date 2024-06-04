using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʽ�̳�Mono�ĵ�������
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
        //�Ѿ����ڵ������󣬲���Ҫ�ٴ�����
        if(instance != null) {
            Destroy(this);
            return;
        }
        instance = this as T;
        //���ؼ̳иõ���ģʽ����Ľű��Ķ�����������Ƴ�����֤��Ϸ�������������ж�����
        DontDestroyOnLoad(this.gameObject);
    }
}
