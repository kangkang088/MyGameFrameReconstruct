using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// UnityWebRequest��Դ����ģ�������
/// </summary>
public class UWQResMgr : SingletonAutoMono<UWQResMgr>
{
    /// <summary>
    /// �첽������Դ��string,byte[],Texture,AssetBundle��
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="callback">������ɻص�</param>
    /// <param name="failureCallback">����ʧ�ܻص�</param>
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

        //������سɹ�
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