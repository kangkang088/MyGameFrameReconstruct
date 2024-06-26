using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽屉对象
/// </summary>
public class PoolData
{
    //抽屉中存储的对象(没有在使用的对象)
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    //使用中的对象
    private List<GameObject> usedList = new List<GameObject>();

    //抽屉根对象
    private GameObject rootObj;

    //抽屉上限  场景中同时存在的对象的上限个数
    private int maxNumber;

    public int Count => dataStack.Count;

    public int UsedCount => usedList.Count;

    public bool NeedCreate => usedList.Count < maxNumber;

    /// <summary>
    /// 初始化抽屉根节点
    /// </summary>
    /// <param name="root">柜子根节点</param>
    /// <param name="name">抽屉根节点名字</param>
    public PoolData(GameObject root,string name,GameObject usedObj)
    {
        if(PoolMgr.isOpenLayout)
        {
            //创建抽屉根节点，并和柜子根节点建立父子关系
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
        PushUsedList(usedObj);

        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if(poolObj == null)
        {
            Debug.LogError("Object has not the script of 'PoolObj'!");
            return;
        }
        maxNumber = poolObj.maxNumber;
    }

    /// <summary>
    /// 抽屉中弹出对象
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        GameObject obj;
        if(Count > 0)
        {
            //从没用的容器中取出对象
            obj = dataStack.Pop();
            //对象记录到使用中容器中
            usedList.Add(obj);
        }
        else
        {
            //使用时间最长的对象
            obj = usedList[0];
            usedList.RemoveAt(0);
            //用完了继续用，但是是最新的了（使用时间最短）
            usedList.Add(obj);
        }

        obj.SetActive(true);
        if(PoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// 往抽屉放回对象
    /// </summary>
    /// <param name="obj">要放回的对象</param>
    public void Push(GameObject obj)
    {
        //失活对象，放入抽屉根节点下，建立父子关系
        obj.SetActive(false);
        if(PoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
        //对象不再使用，从记录容器中移除
        usedList.Remove(obj);
    }

    /// <summary>
    /// 将使用中的对象压入对应容器记录
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/// <summary>
/// 想要被复用的数据结构类和逻辑类，必须继承这个接口
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// 重置数据的方法，保证当对象被放进池子后，数据重置，防止再次取出后遗留上一次的数据
    /// </summary>
    void ResetInfo();
}

public abstract class PoolObjectBase
{
}

/// <summary>
/// 存储 数据结构类和逻辑类(不继承Mono的类) 的容器类
/// </summary>
/// <typeparam name="T">类的类型</typeparam>
public class PoolObject<T> : PoolObjectBase where T : class
{
    public Queue<T> poolObjects = new();
}

public class PoolMgr : BaseManager<PoolMgr>
{
    public static bool isOpenLayout = true;

    private PoolMgr()
    {
    }

    //池子根对象
    private GameObject poolObj;

    /// <summary>
    /// 柜子容器Dictionary，抽屉容器PoolData中的Stack，存储对象GameObject，继承Mono的
    /// </summary>
    private Dictionary<string,PoolData> poolDic = new();

    /// <summary>
    /// 柜子容器Dictionary，抽屉容器PoolObject中的Queue，存储逻辑类和数据结构类，不继承Mono的
    /// </summary>
    private Dictionary<string,PoolObjectBase> poolObjectDic = new();

    /// <summary>
    /// 从缓存池（柜子）取对象的方法
    /// </summary>
    /// <param name="name">抽屉的名字</param>
    /// <returns></returns>
    public GameObject GetObj(string name)
    {
        //从Push中移过来，因为PoolData在这里创建，里面会设置父对象。
        if(poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 没有加入上限时的逻辑

        ////直接拿：有抽屉并且抽屉里面有对象
        //if(poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        //{
        //    obj = poolDic[name].Pop();
        //}
        ////先创建，再拿：没有抽屉或者有抽屉但抽屉里面没对象
        //else
        //{
        //    obj = Object.Instantiate(Resources.Load<GameObject>(name));
        //    obj.name = name;
        //}

        #endregion 没有加入上限时的逻辑

        #region 加入了上限后的逻辑

        //1.没有抽屉
        //if(!poolDic.ContainsKey(name))
        //{
        //    obj = Object.Instantiate(Resources.Load<GameObject>(name));
        //    obj.name = name;
        //    第一次获取的时候就创建抽屉，不在放入的时候创建抽屉了
        //    poolDic.Add(name,new PoolData(poolObj,name,obj));
        //}
        //2.抽屉里有没用的对象或使用中的对象超过上限时(这个条件的判断对应到Pop里面了)
        //else if(poolDic[name].Count > 0 || poolDic[name].UsedCount >= maxNum)
        //{
        //    obj = poolDic[name].Pop();
        //}
        //3.抽屉里面没对象并且使用中的对象也没超过上限
        //else
        //{
        //    obj = Object.Instantiate(Resources.Load<GameObject>(name));
        //    obj.name = name;
        //    记录正在使用的对象
        //    poolDic[name].PushUsedList(obj);
        //}

        #endregion 加入了上限后的逻辑

        #region 加入了上限后的逻辑(优化)

        //1.没有抽屉 || 3.抽屉里面没对象并且使用中的对象也没超过上限
        if(!poolDic.ContainsKey(name) || (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            obj = Object.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;

            if(!poolDic.ContainsKey(name))//第一次获取的时候就创建抽屉，不在放入的时候创建抽屉了
                poolDic.Add(name,new PoolData(poolObj,name,obj));
            else//记录正在使用的对象
                poolDic[name].PushUsedList(obj);
        }
        //2.抽屉里有没用的对象或使用中的对象超过上限时(这个条件的判断对应到Pop里面了)
        else
        {
            obj = poolDic[name].Pop();
        }

        #endregion 加入了上限后的逻辑(优化)

        return obj;
    }

    /// <summary>
    /// 获取自定义数据结构类和逻辑类对象
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="nameSpace">命名空间</param>
    /// <returns>类对象</returns>
    public T GetObj<T>(string nameSpace = "") where T : class, IPoolObject, new()
    {
        //池子名规定：命名空间名 + _ + 类名
        string poolName = nameSpace + "_" + typeof(T).Name;

        //有池子
        if(poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            //池子中受否有可以复用的内容
            if(pool.poolObjects.Count > 0)
            {
                T obj = pool.poolObjects.Dequeue();
                return obj;
            }
            else
            {
                T obj = new();
                return obj;
            }
        }
        //没有池子
        else
        {
            T obj = new();
            return obj;
        }
    }

    /// <summary>
    /// 将自定义数据结构类和逻辑类放入池子
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="obj">对象</param>
    /// <param name="nameSpace">命名空间</param>
    public void PushObj<T>(T obj,string nameSpace = "") where T : class, IPoolObject
    {
        //不允许压入空对象
        if(obj == null)
            return;

        //池子名规定：命名空间名 + _ + 类名
        string poolName = nameSpace + "_" + typeof(T).Name;

        PoolObject<T> pool;

        //有池子
        if(poolObjectDic.ContainsKey(poolName))
        {
            pool = poolObjectDic[poolName] as PoolObject<T>;
        }
        //没有池子
        else
        {
            pool = new();
            poolObjectDic.Add(poolName,pool);
        }

        //放入池子之前，先重置数据
        obj.ResetInfo();
        pool.poolObjects.Enqueue(obj);
    }

    /// <summary>
    /// 往缓存池中放对象的方法
    /// </summary>
    /// <param name="obj">要放入的对象</param>
    public void PushObj(GameObject obj)
    {
        #region 被优化

        //obj.SetActive(false);
        ////建立父子关系
        //obj.transform.SetParent(poolObj.transform);

        #endregion 被优化

        //因为要记录抽屉中使用中的对象，所以抽屉不在这里创建，必须在取对象（没有就创建）的情况下创建抽屉并记录

        //if(!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name,new PoolData(poolObj,obj.name));

        poolDic[obj.name].Push(obj);

        #region 被优化

        ////直接放：有name抽屉
        //if(poolDic.ContainsKey(name))
        //{
        //    poolDic[name].Push(obj);
        //}
        ////先创建抽屉，再放：没有name抽屉
        //else
        //{
        //    poolDic.Add(name,new Stack<GameObject>());
        //    poolDic[name].Push(obj);
        //}

        #endregion 被优化
    }

    /// <summary>
    /// 清空对象池对象引用
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;

        poolObjectDic.Clear();
    }
}