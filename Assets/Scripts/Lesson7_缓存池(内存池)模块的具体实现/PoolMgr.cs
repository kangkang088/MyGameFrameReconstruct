using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 抽屉对象
/// </summary>
public class PoolData {
    //存储抽屉中的对象 记录的是没有使用的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //用来记录使用中对象
    private List<GameObject> usedList = new List<GameObject>();
    //抽屉根对象，用来进行布局管理的对象
    private GameObject rootObj;
    //抽屉上限，场景同时存在的对象上限个数
    private int maxNum;
    public PoolData(GameObject root,string name,GameObject usedObj) {
        if(PoolMgr.isOpenLayout) {
            //创建抽屉父对象
            rootObj = new GameObject(name);
            //和柜子父对象建立父子关系
            rootObj.transform.SetParent(root.transform);
        }
        PushUsedList(usedObj);
        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if(poolObj == null) {
            Debug.Log("Please add 'PoolObj' component for the using object!");
            return;
        }
        maxNum = poolObj.maxNum;
    }
    //获取容器中是否有对象
    public int Count => dataStack.Count;
    public int UsedCount => usedList.Count;
    public int MaxNum => maxNum;

    /// <summary>
    /// 从抽屉中弹出数据对象
    /// </summary>
    /// <returns></returns>
    public GameObject Pop() {
        GameObject obj;
        if(dataStack.Count > 0) {
            obj = dataStack.Pop();
            usedList.Add(obj);
        }
        else {
            obj = usedList[0];
            usedList.RemoveAt(0);
            usedList.Add(obj);
        }
        //激活对象
        obj.SetActive(true);
        if(PoolMgr.isOpenLayout)
            //取出的时候，断开父子关系
            obj.transform.SetParent(null);
        return obj;
    }
    /// <summary>
    /// 将物体放入到抽屉对象中
    /// </summary>
    /// <param name="gameObject"></param>
    public void Push(GameObject gameObject) {
        gameObject.SetActive(false);
        if(PoolMgr.isOpenLayout)
            gameObject.transform.SetParent(rootObj.transform);
        dataStack.Push(gameObject);
        usedList.Remove(gameObject);
    }
    /// <summary>
    /// 将对象压入到记录使用着的对象的容器中
    /// </summary>
    /// <param name="gameObject"></param>
    public void PushUsedList(GameObject gameObject) {
        usedList.Add(gameObject);
    }
}
/// <summary>
/// 缓存池模块管理器
/// </summary>
public class PoolMgr : BaseManager<PoolMgr> {
    /// <summary>
    /// 柜子容器，抽屉为Stack<GameObject>
    /// </summary>
    private Dictionary<string,PoolData> poolDic = new Dictionary<string,PoolData>();
    //池子根对象
    private GameObject poolObj;
    //是否开启布局功能
    public static bool isOpenLayout = true;
    private PoolMgr() {

    }

    /// <summary>
    /// 拿东西的方法
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <returns>从缓存池中取出的对象</returns>
    public GameObject GetGameObject(string name) {
        GameObject obj = null;
        #region 没有加入上限时的逻辑
        ////有抽屉，且抽屉里面有对象，直接拿
        //if(poolDic.ContainsKey(name) && poolDic[name].Count > 0) {
        //    obj = poolDic[name].Pop();

        //    ////激活对象
        //    //obj.SetActive(true);
        //    ////取出的时候，断开父子关系
        //    //obj.transform.SetParent(null);
        //}
        ////没有抽屉或抽屉里面没对象，要创建
        //else {
        //    obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        //    //避免实例化出来的对象，默认后面会有（Clone）
        //    obj.name = name;
        //}
        #endregion
        #region 加入了上限后的逻辑
        if(poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");
        if(!poolDic.ContainsKey(name)) {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //避免实例化出来的对象，默认后面会有（Clone）
            obj.name = name;
            //创建抽屉
            poolDic.Add(name,new PoolData(poolObj,name,obj));
        }
        //当抽屉中有对象，或者使用中的对象超出上限时 直接取出来用
        else if(poolDic[name].Count > 0 || poolDic[name].UsedCount >= poolDic[name].MaxNum) {
            obj = poolDic[name].Pop();
        }
        else {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //避免实例化出来的对象，默认后面会有（Clone）
            obj.name = name;
            //实例化出来的对象记录到使用中对象容器中
            poolDic[name].PushUsedList(obj);
        }
        #endregion

        return obj;
    }
    /// <summary>
    /// 往缓存池中放入对象
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <param name="obj">希望放入的对象</param>
    public void PushGameObject(GameObject obj) {
        ////不直接移除对象，而是失活对象，用的时候再激活
        //obj.SetActive(false);
        ////把失活的对象（要放入抽屉中的对象）父对象设置为柜子对象
        //obj.transform.SetParent(poolObj.transform);


        //没有抽屉容器，创建抽屉
        //if(!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name,new PoolData(poolObj,obj.name));
        //存在对应的抽屉容器，直接放入
        poolDic[obj.name].Push(obj);
    }
    /// <summary>
    /// 清除整个柜子中的对象
    /// 主要在切换场景时调用
    /// </summary>
    public void ClearPool() {
        poolDic.Clear();
        poolObj = null;
    }
}
