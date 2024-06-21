using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ResInfoBase
{
}

/// <summary>
/// ��Դ��Ϣ�࣬�洢��Դ��Ϣ���첽���ص�ί�У�Эͬ�������
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResInfo<T> : ResInfoBase
{
    //��Դ
    public T asset;

    //�첽���ؽ������ί�У�����Դ���ݳ�ȥ
    public UnityAction<T> callback;

    //Э�̶���,�洢�첽����ʱЭͬ�����ķ��ض���
    public Coroutine coroutine;

    //�Ƿ���Ҫ�Ƴ�
    public bool isDel;

    //���ü���
    public int refCount;

    public void AddRefCount()
    {
        refCount++;
    }

    public void SubRefCount()
    {
        refCount--;
        if(refCount < 0)
        {
            Debug.LogError("Reference_Count has been less than zero!");
        }
    }
}

/// <summary>
/// Resources ��Դ����ģ�������
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //�洢���ع��ģ��Լ������е���Դ������
    private Dictionary<string,ResInfoBase> resDic = new();

    private ResMgr()
    {
    }

    /// <summary>
    /// ͬ��������Դ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <returns>��Դ</returns>
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> resInfo;

        if(!resDic.ContainsKey(resName))
        {
            T res = Resources.Load<T>(path);
            resInfo = new ResInfo<T>
            {
                asset = res
            };
            //���ü�������
            resInfo.AddRefCount();
            resDic.Add(resName,resInfo);
            return res;
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<T>;
            //���ü�������
            resInfo.AddRefCount();
            //�첽���أ����ڼ�����
            if(resInfo.asset == null)
            {
                //ֹͣ�첽���أ�ֱ�Ӳ���ͬ�����ط�ʽ������¼��ʹ��
                MonoMgr.Instance.StopCoroutine(resInfo.coroutine);

                //ͬ������
                T res = Resources.Load<T>(path);
                //��¼
                resInfo.asset = res;
                //�ѵȴ����첽������ɵĻص�����ִ�е�
                resInfo.callback?.Invoke(res);
                //�������
                resInfo.callback = null;
                resInfo.coroutine = null;
                //ʹ��
                return res;
            }
            else
            {
                return resInfo.asset;
            }
        }
    }

    /// <summary>
    /// �첽������Դ�ķ���
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="callback">���ؽ�����Ļص����첽���ؽ��������</param>
    public void LoadAsync<T>(string path,UnityAction<T> callback) where T : UnityEngine.Object
    {
        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path,callback));

        //��ԴΨһID : ·��_������
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> resInfo;
        if(!resDic.ContainsKey(resName))
        {
            resInfo = new();
            //���ü�������
            resInfo.AddRefCount();
            resDic.Add(resName,resInfo);
            resInfo.callback += callback;
            //ע�⣬��һ֡����ʱ������Դ��û�м��سɹ���
            resInfo.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<T>;
            //���ü�������
            resInfo.AddRefCount();
            //��Դû�м�����
            if(resInfo.asset == null)
            {
                //�Ѿ�����Э���ˣ��ڼ�����Դ�С��Ͳ�Ҫ�ٿ��ˣ�ֱ�Ӽ�¼
                resInfo.callback += callback;
            }
            else
            {
                callback?.Invoke(resInfo.asset);
            }
        }
    }

    private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
    {
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        yield return rq;
        //��ԴΨһID : ·��_������
        string resName = path + "_" + typeof(T).Name;
        if(resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //ȡ����Դ��Ϣ������¼
            resInfo.asset = rq.asset as T;

            //�����Դ�����ü���Ϊ0����ɾ����Դ
            if(resInfo.refCount == 0)
            {
                UnloadAsset<T>(path);
            }
            else
            {
                //��������ɵ���Դ���ݳ�ȥ
                resInfo.callback?.Invoke(resInfo.asset);
                //������Ϻ�������ã������ڴ�й©
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// �첽������Դ�ķ���
    /// </summary>
    /// <param name="path">��Դ·��</param>
    /// <param name="type">��Դ����</param>
    /// <param name="callback">���ؽ�����Ļص����첽���ؽ��������</param>
    [Obsolete("��ʹ�÷��ͷ�ʽ���ء�����ֻѡ��һ�ֶ��ǻ�ϵķ�ʽ������Դ")]
    public void LoadAsync(string path,Type type,UnityAction<UnityEngine.Object> callback)
    {
        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path,type,callback));

        //��ԴΨһID : ·��_������
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> resInfo;
        if(!resDic.ContainsKey(resName))
        {
            resInfo = new();
            //���ü�������
            resInfo.AddRefCount();
            resDic.Add(resName,resInfo);
            resInfo.callback += callback;
            //ע�⣬��һ֡����ʱ������Դ��û�м��سɹ���
            resInfo.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path,type));
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //���ü�������
            resInfo.AddRefCount();
            //��Դû�м�����
            if(resInfo.asset == null)
            {
                //�Ѿ�����Э���ˣ��ڼ�����Դ�С��Ͳ�Ҫ�ٿ��ˣ�ֱ�Ӽ�¼
                resInfo.callback += callback;
            }
            else
            {
                callback?.Invoke(resInfo.asset);
            }
        }
    }

    private IEnumerator ReallyLoadAsync(string path,Type type)
    {
        ResourceRequest rq = Resources.LoadAsync(path,type);
        yield return rq;
        //��ԴΨһID : ·��_������
        string resName = path + "_" + type.Name;
        if(resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //ȡ����Դ��Ϣ������¼
            resInfo.asset = rq.asset;

            //�����Դ�����ü���Ϊ0����ɾ����Դ
            if(resInfo.refCount == 0)
            {
                UnloadAsset(path,type);
            }
            else
            {
                //��������ɵ���Դ���ݳ�ȥ
                resInfo.callback?.Invoke(resInfo.asset);
                //������Ϻ�������ã������ڴ�й©
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// ж��ָ����Դ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    public void UnloadAsset<T>(string path,UnityAction<T> callback = null)
    {
        string resName = path + "_" + typeof(T).Name;

        //��Դ�Ѿ���ʼ�����ˣ�������֪����
        if(resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;

            //���ü�����һ
            resInfo.SubRefCount();

            //��Դ���������
            if(resInfo.asset != null)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            //��Դ������
            else
            {
                //���������գ���Ϊ���ǲ�ȷ��Э�̵�ʲô�ز��ˣ���Դ�����Ѿ���������ˣ�Ҳ����û��ɡ�
                //MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);

                //��Դ�����У���ɾ��
                //resInfo.isDel = true;

                //��Դ�����У���һ�εļ��ز���Ҫ�ˣ���ȷ����Դ��û�ñ���ĵط�ʹ�ã���ȥ����εĻص���
                if(resInfo.callback != null)
                    resInfo.callback -= callback;
            }
        }
    }

    public void UnloadAsset(string path,Type type,UnityAction<UnityEngine.Object> callback = null)
    {
        string resName = path + "_" + type.Name;

        //��Դ�Ѿ���ʼ�����ˣ�������֪����
        if(resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;

            //���ü�����һ
            resInfo.SubRefCount();

            //��Դ���������
            if(resInfo.asset != null)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            //��Դ������
            else
            {
                //���������գ���Ϊ���ǲ�ȷ��Э�̵�ʲô�ز��ˣ���Դ�����Ѿ���������ˣ�Ҳ����û��ɡ�
                //MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);

                //��Դ�����У���ɾ��
                //resInfo.isDel = true;

                //��Դ�����У���һ�εļ��ز���Ҫ�ˣ���ȷ����Դ��û�ñ���ĵط�ʹ�ã���ȥ����εĻص���
                if(resInfo.callback != null)
                    resInfo.callback -= callback;
            }
        }
    }

    /// <summary>
    /// �첽ж������û��ʹ�õ���Դ
    /// </summary>
    /// <param name="callback">ж����Ϻ�Ļص�����</param>
    public void UnloadUnusedAssets(UnityAction callback)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callback));
    }

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callback)
    {
        AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();
        yield return asyncOperation;
        //ж����Ϻ�֪ͨ�ⲿ
        callback();
    }
}