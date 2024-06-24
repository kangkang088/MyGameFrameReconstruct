using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// UnityWebRequest资源加载模块管理器
/// </summary>
public class UWQResMgr : SingletonAutoMono<UWQResMgr>
{
    /// <summary>
    /// 异步加载资源（string,byte[],Texture,AssetBundle）
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="callback">加载完成回调</param>
    /// <param name="failureCallback">加载失败回调</param>
    public void LoadRes<T>(string path,UnityAction<T> callback,UnityAction failureCallback) where T : class
    {
        StartCoroutine(ReallyLoadRes(path,callback,failureCallback));
    }

    private IEnumerator ReallyLoadRes<T>(string path,UnityAction<T> callback,UnityAction failureCallback) where T : class
    {
        //T:string,byte[],Texture,AssetBundle

        Type type = typeof(T);
        UnityWebRequest req = null;

        if(type == typeof(string) || type == typeof(byte[]))
            req = UnityWebRequest.Get(path);
        else if(type == typeof(Texture))
            req = UnityWebRequestTexture.GetTexture(path);
        else if(type == typeof(AssetBundle))
            req = UnityWebRequestAssetBundle.GetAssetBundle(path);
        else
            yield break;

        yield return req.SendWebRequest();

        //如果加载成功
        if(req.result == UnityWebRequest.Result.Success)
        {
            if(type == typeof(string))
                callback?.Invoke(req.downloadHandler.text as T);
            else if(type == typeof(byte[]))
                callback?.Invoke(req.downloadHandler.data as T);
            else if(type == typeof(Texture))
                callback?.Invoke(DownloadHandlerTexture.GetContent(req) as T);
            else if(type == typeof(AssetBundle))
                callback?.Invoke(DownloadHandlerAssetBundle.GetContent(req) as T);
        }
        else
            failureCallback?.Invoke();

        req.Dispose();
    }
}