using UnityEngine;

/// <summary>
/// 继承Mono的单例模式基类，自动挂载式。
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj = new GameObject
                {
                    name = typeof(T).ToString()
                };
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}