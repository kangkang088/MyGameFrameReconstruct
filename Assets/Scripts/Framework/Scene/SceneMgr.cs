using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块管理器
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr()
    {
    }

    /// <summary>
    /// 同步切换场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="callback">切换完毕回调</param>
    public void LoadScene(string sceneName,UnityAction callback = null)
    {
        SceneManager.LoadScene(sceneName);
        callback?.Invoke();
    }

    /// <summary>
    /// 异步切换场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="callback">加载完成的回调</param>
    public void LoadSceneAsync(string sceneName,UnityAction callback = null)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName,callback));
    }

    private IEnumerator ReallyLoadSceneAsync(string sceneName,UnityAction callback = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        //不停的在协同程序中每帧检测是否加载结束
        while(!ao.isDone)
        {
            //利用事件中心每一帧将进度发送给想要得到的地方
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneProgress_Change,ao.progress);
            yield return 0;
        }
        //避免最后一帧直接结束，没传1
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneProgress_Change,1);
        yield return ao;
        callback?.Invoke();
    }
}