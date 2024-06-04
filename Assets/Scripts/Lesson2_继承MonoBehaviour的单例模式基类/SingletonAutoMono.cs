using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;
    public static T Instance {
        get {
            if(instance == null) {
                //在场景上创建空物体
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
