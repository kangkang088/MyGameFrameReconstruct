using UnityEngine;

/// <summary>
/// �̳�Mono�ĵ���ģʽ���࣬�Զ�����ʽ��
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