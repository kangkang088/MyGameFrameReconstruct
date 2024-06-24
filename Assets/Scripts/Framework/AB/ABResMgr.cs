using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ABMgr和EditorResMgr整合管理器
/// </summary>
public class ABResMgr : BaseManager<ABResMgr>
{
    /// <summary>
    /// true:Editor false:AssetBundle
    /// </summary>
    private bool isDebug = false;

    private ABResMgr()
    {
    }

    public void LoadResAsync<T>(string abName,string resName,UnityAction<T> callBack,bool isSync = false) where T : Object
    {
#if UNITY_EDITOR
        if(isDebug)
        {
            //自定义了一个规则：文件夹名为包名，为了让Editor和AB适配
            T res = EditorResMgr.Instance.LoadEditorRes<T>($"{abName}/{resName}");
            callBack?.Invoke(res);
        }
        else
        {
            ABMgr.Instance.LoadResAsync<T>(abName,resName,callBack,isSync);
        }
#else
        ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
#endif
    }
}