using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �����л�ģ�������
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr()
    {
    }

    /// <summary>
    /// ͬ���л�����
    /// </summary>
    /// <param name="sceneName">������</param>
    /// <param name="callback">�л���ϻص�</param>
    public void LoadScene(string sceneName,UnityAction callback = null)
    {
        SceneManager.LoadScene(sceneName);
        callback?.Invoke();
    }

    /// <summary>
    /// �첽�л�����
    /// </summary>
    /// <param name="sceneName">������</param>
    /// <param name="callback">������ɵĻص�</param>
    public void LoadSceneAsync(string sceneName,UnityAction callback = null)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName,callback));
    }

    private IEnumerator ReallyLoadSceneAsync(string sceneName,UnityAction callback = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        //��ͣ����Эͬ������ÿ֡����Ƿ���ؽ���
        while(!ao.isDone)
        {
            //�����¼�����ÿһ֡�����ȷ��͸���Ҫ�õ��ĵط�
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneProgress_Change,ao.progress);
            yield return 0;
        }
        //�������һֱ֡�ӽ�����û��1
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneProgress_Change,1);
        yield return ao;
        callback?.Invoke();
    }
}