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

public class PoolMgr : BaseManager<PoolMgr>
{
    public static bool isOpenLayout = true;

    private PoolMgr()
    {
    }

    //池子根对象
    private GameObject poolObj;

    /// <summary>
    /// 柜子容器Dictionary，抽屉容器Stack，存储对象GameObject
    /// </summary>
    private Dictionary<string,PoolData> poolDic = new Dictionary<string,PoolData>();

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
        #endregion

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
        #endregion

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
        #endregion

        return obj;
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
        #endregion

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
        #endregion
    }

    /// <summary>
    /// 清空对象池对象引用
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
    }
}