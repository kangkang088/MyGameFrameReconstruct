using UnityEngine;

/// <summary>
/// �̳�Mono�ĵ���ģʽ���࣬�ֶ�����ʽ��
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