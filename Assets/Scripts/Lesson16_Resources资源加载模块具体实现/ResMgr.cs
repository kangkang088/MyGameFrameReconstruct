using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ResInfoBase
{
}

/// <summary>
/// 资源信息类，存储资源信息，异步加载的委托，协同程序对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResInfo<T> : ResInfoBase
{
    //资源
    public T asset;

    //异步加载结束后的委托，将资源传递出去
    public UnityAction<T> callback;

    //协程对象,存储异步加载时协同函数的返回对象
    public Coroutine coroutine;

    //是否需要移除
    public bool isDel;

    //引用计数
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
/// Resources 资源加载模块管理器
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //存储加载过的，以及加载中的资源的容器
    private Dictionary<string,ResInfoBase> resDic = new();

    private ResMgr()
    {
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <returns>资源</returns>
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
            //引用计数增加
            resInfo.AddRefCount();
            resDic.Add(resName,resInfo);
            return res;
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<T>;
            //引用计数增加
            resInfo.AddRefCount();
            //异步加载，还在加载中
            if(resInfo.asset == null)
            {
                //停止异步加载，直接采用同步加载方式，并记录，使用
                MonoMgr.Instance.StopCoroutine(resInfo.coroutine);

                //同步加载
                T res = Resources.Load<T>(path);
                //记录
                resInfo.asset = res;
                //把等待着异步加载完成的回调函数执行掉
                resInfo.callback?.Invoke(res);
                //清空引用
                resInfo.callback = null;
                resInfo.coroutine = null;
                //使用
                return res;
            }
            else
            {
                return resInfo.asset;
            }
        }
    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="callback">加载结束后的回调，异步加载结束后调用</param>
    public void LoadAsync<T>(string path,UnityAction<T> callback) where T : UnityEngine.Object
    {
        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path,callback));

        //资源唯一ID : 路径_类型名
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> resInfo;
        if(!resDic.ContainsKey(resName))
        {
            resInfo = new();
            //引用计数增加
            resInfo.AddRefCount();
            resDic.Add(resName,resInfo);
            resInfo.callback += callback;
            //注意，这一帧（此时），资源是没有加载成功的
            resInfo.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<T>;
            //引用计数增加
            resInfo.AddRefCount();
            //资源没有加载完
            if(resInfo.asset == null)
            {
                //已经开启协程了，在加载资源中。就不要再开了，直接记录
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
        //资源唯一ID : 路径_类型名
        string resName = path + "_" + typeof(T).Name;
        if(resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //取出资源信息，并记录
            resInfo.asset = rq.asset as T;

            //如果资源的引用计数为0，则删除资源
            if(resInfo.refCount == 0)
            {
                UnloadAsset<T>(path);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callback?.Invoke(resInfo.asset);
                //加载完毕后，清空引用，避免内存泄漏
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型</param>
    /// <param name="callback">加载结束后的回调，异步加载结束后调用</param>
    [Obsolete("请使用泛型方式加载。或者只选择一种而非混合的方式加载资源")]
    public void LoadAsync(string path,Type type,UnityAction<UnityEngine.Object> callback)
    {
        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path,type,callback));

        //资源唯一ID : 路径_类型名
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> resInfo;
        if(!resDic.ContainsKey(resName))
        {
            resInfo = new();
            //引用计数增加
            resInfo.AddRefCount();
            resDic.Add(resName,resInfo);
            resInfo.callback += callback;
            //注意，这一帧（此时），资源是没有加载成功的
            resInfo.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path,type));
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数增加
            resInfo.AddRefCount();
            //资源没有加载完
            if(resInfo.asset == null)
            {
                //已经开启协程了，在加载资源中。就不要再开了，直接记录
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
        //资源唯一ID : 路径_类型名
        string resName = path + "_" + type.Name;
        if(resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //取出资源信息，并记录
            resInfo.asset = rq.asset;

            //如果资源的引用计数为0，则删除资源
            if(resInfo.refCount == 0)
            {
                UnloadAsset(path,type);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callback?.Invoke(resInfo.asset);
                //加载完毕后，清空引用，避免内存泄漏
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// 卸载指定资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    public void UnloadAsset<T>(string path,UnityAction<T> callback = null)
    {
        string resName = path + "_" + typeof(T).Name;

        //资源已经开始加载了（完成与否不知道）
        if(resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;

            //引用计数减一
            resInfo.SubRefCount();

            //资源加载完成了
            if(resInfo.asset != null)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            //资源加载中
            else
            {
                //这样不保险，因为我们不确定协程到什么地步了，资源可能已经加载完成了，也可能没完成。
                //MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);

                //资源加载中，待删除
                //resInfo.isDel = true;

                //资源加载中，这一次的加载不想要了，不确定资源有没用被别的地方使用，先去掉这次的回调。
                if(resInfo.callback != null)
                    resInfo.callback -= callback;
            }
        }
    }

    public void UnloadAsset(string path,Type type,UnityAction<UnityEngine.Object> callback = null)
    {
        string resName = path + "_" + type.Name;

        //资源已经开始加载了（完成与否不知道）
        if(resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;

            //引用计数减一
            resInfo.SubRefCount();

            //资源加载完成了
            if(resInfo.asset != null)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            //资源加载中
            else
            {
                //这样不保险，因为我们不确定协程到什么地步了，资源可能已经加载完成了，也可能没完成。
                //MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);

                //资源加载中，待删除
                //resInfo.isDel = true;

                //资源加载中，这一次的加载不想要了，不确定资源有没用被别的地方使用，先去掉这次的回调。
                if(resInfo.callback != null)
                    resInfo.callback -= callback;
            }
        }
    }

    /// <summary>
    /// 异步卸载所有没有使用的资源
    /// </summary>
    /// <param name="callback">卸载完毕后的回调函数</param>
    public void UnloadUnusedAssets(UnityAction callback)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callback));
    }

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callback)
    {
        AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();
        yield return asyncOperation;
        //卸载完毕后通知外部
        callback();
    }
}