using UnityEngine;

/// <summary>
/// 继承Mono的单例模式基类，手动挂载式。
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance != null)
            Destroy(this);
        instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}