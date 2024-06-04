using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �������
/// </summary>
public class PoolData {
    //�洢�����еĶ��� ��¼����û��ʹ�õĶ���
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //������¼ʹ���ж���
    private List<GameObject> usedList = new List<GameObject>();
    //����������������в��ֹ���Ķ���
    private GameObject rootObj;
    //�������ޣ�����ͬʱ���ڵĶ������޸���
    private int maxNum;
    public PoolData(GameObject root,string name,GameObject usedObj) {
        if(PoolMgr.isOpenLayout) {
            //�������븸����
            rootObj = new GameObject(name);
            //�͹��Ӹ����������ӹ�ϵ
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
    //��ȡ�������Ƿ��ж���
    public int Count => dataStack.Count;
    public int UsedCount => usedList.Count;
    public int MaxNum => maxNum;

    /// <summary>
    /// �ӳ����е������ݶ���
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
        //�������
        obj.SetActive(true);
        if(PoolMgr.isOpenLayout)
            //ȡ����ʱ�򣬶Ͽ����ӹ�ϵ
            obj.transform.SetParent(null);
        return obj;
    }
    /// <summary>
    /// ��������뵽���������
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
    /// ������ѹ�뵽��¼ʹ���ŵĶ����������
    /// </summary>
    /// <param name="gameObject"></param>
    public void PushUsedList(GameObject gameObject) {
        usedList.Add(gameObject);
    }
}
/// <summary>
/// �����ģ�������
/// </summary>
public class PoolMgr : BaseManager<PoolMgr> {
    /// <summary>
    /// ��������������ΪStack<GameObject>
    /// </summary>
    private Dictionary<string,PoolData> poolDic = new Dictionary<string,PoolData>();
    //���Ӹ�����
    private GameObject poolObj;
    //�Ƿ������ֹ���
    public static bool isOpenLayout = true;
    private PoolMgr() {

    }

    /// <summary>
    /// �ö����ķ���
    /// </summary>
    /// <param name="name">��������������</param>
    /// <returns>�ӻ������ȡ���Ķ���</returns>
    public GameObject GetGameObject(string name) {
        GameObject obj = null;
        #region û�м�������ʱ���߼�
        ////�г��룬�ҳ��������ж���ֱ����
        //if(poolDic.ContainsKey(name) && poolDic[name].Count > 0) {
        //    obj = poolDic[name].Pop();

        //    ////�������
        //    //obj.SetActive(true);
        //    ////ȡ����ʱ�򣬶Ͽ����ӹ�ϵ
        //    //obj.transform.SetParent(null);
        //}
        ////û�г�����������û����Ҫ����
        //else {
        //    obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        //    //����ʵ���������Ķ���Ĭ�Ϻ�����У�Clone��
        //    obj.name = name;
        //}
        #endregion
        #region ���������޺���߼�
        if(poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");
        if(!poolDic.ContainsKey(name)) {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //����ʵ���������Ķ���Ĭ�Ϻ�����У�Clone��
            obj.name = name;
            //��������
            poolDic.Add(name,new PoolData(poolObj,name,obj));
        }
        //���������ж��󣬻���ʹ���еĶ��󳬳�����ʱ ֱ��ȡ������
        else if(poolDic[name].Count > 0 || poolDic[name].UsedCount >= poolDic[name].MaxNum) {
            obj = poolDic[name].Pop();
        }
        else {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //����ʵ���������Ķ���Ĭ�Ϻ�����У�Clone��
            obj.name = name;
            //ʵ���������Ķ����¼��ʹ���ж���������
            poolDic[name].PushUsedList(obj);
        }
        #endregion

        return obj;
    }
    /// <summary>
    /// ��������з������
    /// </summary>
    /// <param name="name">��������������</param>
    /// <param name="obj">ϣ������Ķ���</param>
    public void PushGameObject(GameObject obj) {
        ////��ֱ���Ƴ����󣬶���ʧ������õ�ʱ���ټ���
        //obj.SetActive(false);
        ////��ʧ��Ķ���Ҫ��������еĶ��󣩸���������Ϊ���Ӷ���
        //obj.transform.SetParent(poolObj.transform);


        //û�г�����������������
        //if(!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name,new PoolData(poolObj,obj.name));
        //���ڶ�Ӧ�ĳ���������ֱ�ӷ���
        poolDic[obj.name].Push(obj);
    }
    /// <summary>
    /// ������������еĶ���
    /// ��Ҫ���л�����ʱ����
    /// </summary>
    public void ClearPool() {
        poolDic.Clear();
        poolObj = null;
    }
}
