using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//知识点
//字典
//协程
//AB包相关API
//委托
//lambda表达式
//单例模式基类——>观看Unity小框架视频 进行学习
public class ABMgr : SingletonAutoMono<ABMgr>
{
    //主包
    private AssetBundle mainAB = null;

    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    //AB包不能够重复加载 否则会报错
    //字典知识 用来存储 AB包对象
    private Dictionary<string,AssetBundle> abDic = new Dictionary<string,AssetBundle>();

    /// <summary>
    /// 获取AB包加载路径
    /// </summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 加载主包 和 配置文件
    /// 因为加载所有包是 都得判断 通过它才能得到依赖信息
    /// 所以写一个方法
    /// </summary>
    private void LoadMainAB()
    {
        if(mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    /// <summary>
    /// 加载指定包的依赖包
    /// </summary>
    /// <param name="abName"></param>
    private void LoadDependencies(string abName)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for(int i = 0;i < strs.Length;i++)
        {
            if(!abDic.ContainsKey(strs[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i],ab);
            }
        }
    }

    #region 所有同步代码注释掉

    ///// <summary>
    ///// 泛型资源同步加载
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <returns></returns>
    //public T LoadRes<T>(string abName,string resName) where T : Object
    //{
    //    //加载依赖包
    //    LoadDependencies(abName);
    //    //加载目标包
    //    if(!abDic.ContainsKey(abName))
    //    {
    //        AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
    //        abDic.Add(abName,ab);
    //    }

    //    //得到加载出来的资源
    //    T obj = abDic[abName].LoadAsset<T>(resName);
    //    //如果是GameObject 因为GameObject 100%都是需要实例化的
    //    //所以我们直接实例化
    //    if(obj is GameObject)
    //        return Instantiate(obj);
    //    else
    //        return obj;
    //}

    ///// <summary>
    ///// Type同步加载指定资源
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public Object LoadRes(string abName,string resName,System.Type type)
    //{
    //    //加载依赖包
    //    LoadDependencies(abName);
    //    //加载目标包
    //    if(!abDic.ContainsKey(abName))
    //    {
    //        AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
    //        abDic.Add(abName,ab);
    //    }

    //    //得到加载出来的资源
    //    Object obj = abDic[abName].LoadAsset(resName,type);
    //    //如果是GameObject 因为GameObject 100%都是需要实例化的
    //    //所以我们直接实例化
    //    if(obj is GameObject)
    //        return Instantiate(obj);
    //    else
    //        return obj;
    //}

    ///// <summary>
    ///// 名字 同步加载指定资源
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <returns></returns>
    //public Object LoadRes(string abName,string resName)
    //{
    //    //加载依赖包
    //    LoadDependencies(abName);
    //    //加载目标包
    //    if(!abDic.ContainsKey(abName))
    //    {
    //        AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
    //        abDic.Add(abName,ab);
    //    }

    //    //得到加载出来的资源
    //    Object obj = abDic[abName].LoadAsset(resName);
    //    //如果是GameObject 因为GameObject 100%都是需要实例化的
    //    //所以我们直接实例化
    //    if(obj is GameObject)
    //        return Instantiate(obj);
    //    else
    //        return obj;
    //}

    #endregion 所有同步代码注释掉

    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    /// <param name="isSync">是否同步</param>
    public void LoadResAsync<T>(string abName,string resName,UnityAction<T> callBack,bool isSync = false) where T : Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName,resName,callBack,isSync));
    }

    //正儿八经的 协程函数
    private IEnumerator ReallyLoadResAsync<T>(string abName,string resName,UnityAction<T> callBack,bool isSync) where T : Object
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for(int i = 0;i < strs.Length;i++)
        {
            //还没加载过AB包
            if(!abDic.ContainsKey(strs[i]))
            {
                //同步
                if(isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i],ab);
                }
                //异步加载
                else
                {
                    //开始异步加载就记录，如果为空，说明正在异步加载，如果不为空，说明异步加载已经结束了
                    abDic.Add(strs[i],null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    //异步加载结束
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //字典中已经记录AB包相关信息了，具体有没用加载完，不知道。
            else
            {
                //null，说明还在异步加载
                while(abDic[strs[i]] == null)
                {
                    //不停的等待一帧，下一帧再进行判断有没用加载完
                    yield return 0;
                }
                //不为null，说明已经加载完了
            }
        }

        //加载目标包
        if(!abDic.ContainsKey(abName))
        {
            //同步
            if(isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName,ab);
            }
            //异步加载
            else
            {
                //开始异步加载就记录，如果为空，说明正在异步加载，如果不为空，说明异步加载已经结束了
                abDic.Add(abName,null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                //异步加载结束
                abDic[abName] = req.assetBundle;
            }
        }
        //字典中已经记录AB包相关信息了，具体有没用加载完，不知道。
        else
        {
            //null，说明还在异步加载
            while(abDic[abName] == null)
            {
                //不停的等待一帧，下一帧再进行判断有没用加载完
                yield return 0;
            }
            //不为null，说明已经加载完了
        }

        //同步加载AB包中的资源
        if(isSync)
        {
            //同步加载也需要回调把资源穿给外部使用
            T res = abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        //异步加载包中资源
        else
        {
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abq;
            callBack(abq.asset as T);
        }
    }

    /// <summary>
    /// Type异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName,string resName,System.Type type,UnityAction<Object> callBack,bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName,resName,type,callBack,isSync));
    }

    private IEnumerator ReallyLoadResAsync(string abName,string resName,System.Type type,UnityAction<Object> callBack,bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for(int i = 0;i < strs.Length;i++)
        {
            //还没加载过AB包
            if(!abDic.ContainsKey(strs[i]))
            {
                //同步
                if(isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i],ab);
                }
                //异步加载
                else
                {
                    //开始异步加载就记录，如果为空，说明正在异步加载，如果不为空，说明异步加载已经结束了
                    abDic.Add(strs[i],null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    //异步加载结束
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //字典中已经记录AB包相关信息了，具体有没用加载完，不知道。
            else
            {
                //null，说明还在异步加载
                while(abDic[strs[i]] == null)
                {
                    //不停的等待一帧，下一帧再进行判断有没用加载完
                    yield return 0;
                }
                //不为null，说明已经加载完了
            }
        }

        //加载目标包
        if(!abDic.ContainsKey(abName))
        {
            //同步
            if(isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName,ab);
            }
            //异步加载
            else
            {
                //开始异步加载就记录，如果为空，说明正在异步加载，如果不为空，说明异步加载已经结束了
                abDic.Add(abName,null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                //异步加载结束
                abDic[abName] = req.assetBundle;
            }
        }
        //字典中已经记录AB包相关信息了，具体有没用加载完，不知道。
        else
        {
            //null，说明还在异步加载
            while(abDic[abName] == null)
            {
                //不停的等待一帧，下一帧再进行判断有没用加载完
                yield return 0;
            }
            //不为null，说明已经加载完了
        }

        if(isSync)
        {
            Object res = abDic[abName].LoadAsset(resName,type);
            callBack(res);
        }
        //异步加载包中资源
        else
        {
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName,type);
            yield return abq;

            callBack(abq.asset);
        }
    }

    /// <summary>
    /// 名字 异步加载 指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName,string resName,UnityAction<Object> callBack,bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName,resName,callBack,isSync));
    }

    private IEnumerator ReallyLoadResAsync(string abName,string resName,UnityAction<Object> callBack,bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for(int i = 0;i < strs.Length;i++)
        {
            //还没加载过AB包
            if(!abDic.ContainsKey(strs[i]))
            {
                //同步
                if(isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i],ab);
                }
                //异步加载
                else
                {
                    //开始异步加载就记录，如果为空，说明正在异步加载，如果不为空，说明异步加载已经结束了
                    abDic.Add(strs[i],null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    //异步加载结束
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //字典中已经记录AB包相关信息了，具体有没用加载完，不知道。
            else
            {
                //null，说明还在异步加载
                while(abDic[strs[i]] == null)
                {
                    //不停的等待一帧，下一帧再进行判断有没用加载完
                    yield return 0;
                }
                //不为null，说明已经加载完了
            }
        }

        //加载目标包
        if(!abDic.ContainsKey(abName))
        {
            //同步
            if(isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName,ab);
            }
            //异步加载
            else
            {
                //开始异步加载就记录，如果为空，说明正在异步加载，如果不为空，说明异步加载已经结束了
                abDic.Add(abName,null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                //异步加载结束
                abDic[abName] = req.assetBundle;
            }
        }
        //字典中已经记录AB包相关信息了，具体有没用加载完，不知道。
        else
        {
            //null，说明还在异步加载
            while(abDic[abName] == null)
            {
                //不停的等待一帧，下一帧再进行判断有没用加载完
                yield return 0;
            }
            //不为null，说明已经加载完了
        }

        if(isSync)
        {
            Object res = abDic[abName].LoadAsset(resName);
            callBack(res);
        }
        //异步加载包中资源
        else
        {
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName);
            yield return abq;
            callBack(abq.asset);
        }
    }

    //卸载AB包的方法
    public void UnLoadAB(string name,UnityAction<bool> callback)
    {
        if(abDic.ContainsKey(name))
        {
            if(abDic[name] == null)
            {
                //正在异步加载，没有卸载成功
                callback(false);
                return;
            }
            abDic[name].Unload(false);
            abDic.Remove(name);

            //卸载成功
            callback(true);
        }
    }

    //清空AB包的方法
    public void ClearAB()
    {
        //AB包都是异步加载的，清理前，停止所有协同程序
        StopAllCoroutines();

        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        //卸载主包
        mainAB = null;
    }
}