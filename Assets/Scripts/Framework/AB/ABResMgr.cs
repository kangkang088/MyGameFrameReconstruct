using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ABMgr��EditorResMgr���Ϲ�����
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
            //�Զ�����һ�������ļ�����Ϊ������Ϊ����Editor��AB����
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