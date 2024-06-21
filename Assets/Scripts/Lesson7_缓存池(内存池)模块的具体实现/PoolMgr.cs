using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������
/// </summary>
public class PoolData
{
    //�����д洢�Ķ���(û����ʹ�õĶ���)
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    //ʹ���еĶ���
    private List<GameObject> usedList = new List<GameObject>();

    //���������
    private GameObject rootObj;

    //��������  ������ͬʱ���ڵĶ�������޸���
    private int maxNumber;

    public int Count => dataStack.Count;

    public int UsedCount => usedList.Count;

    public bool NeedCreate => usedList.Count < maxNumber;

    /// <summary>
    /// ��ʼ��������ڵ�
    /// </summary>
    /// <param name="root">���Ӹ��ڵ�</param>
    /// <param name="name">������ڵ�����</param>
    public PoolData(GameObject root,string name,GameObject usedObj)
    {
        if(PoolMgr.isOpenLayout)
        {
            //����������ڵ㣬���͹��Ӹ��ڵ㽨�����ӹ�ϵ
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
    /// �����е�������
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        GameObject obj;
        if(Count > 0)
        {
            //��û�õ�������ȡ������
            obj = dataStack.Pop();
            //�����¼��ʹ����������
            usedList.Add(obj);
        }
        else
        {
            //ʹ��ʱ����Ķ���
            obj = usedList[0];
            usedList.RemoveAt(0);
            //�����˼����ã����������µ��ˣ�ʹ��ʱ����̣�
            usedList.Add(obj);
        }

        obj.SetActive(true);
        if(PoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }
    
    /// <summary>
    /// ������Żض���
    /// </summary>
    /// <param name="obj">Ҫ�ŻصĶ���</param>
    public void Push(GameObject obj)
    {
        //ʧ����󣬷��������ڵ��£��������ӹ�ϵ
        obj.SetActive(false);
        if(PoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
        //������ʹ�ã��Ӽ�¼�������Ƴ�
        usedList.Remove(obj);
    }

    /// <summary>
    /// ��ʹ���еĶ���ѹ���Ӧ������¼
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

    //���Ӹ�����
    private GameObject poolObj;

    /// <summary>
    /// ��������Dictionary����������Stack���洢����GameObject
    /// </summary>
    private Dictionary<string,PoolData> poolDic = new Dictionary<string,PoolData>();

    /// <summary>
    /// �ӻ���أ����ӣ�ȡ����ķ���
    /// </summary>
    /// <param name="name">���������</param>
    /// <returns></returns>
    public GameObject GetObj(string name)
    {
        //��Push���ƹ�������ΪPoolData�����ﴴ������������ø�����
        if(poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;
        #region û�м�������ʱ���߼�
        ////ֱ���ã��г��벢�ҳ��������ж���
        //if(poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        //{
        //    obj = poolDic[name].Pop();
        //}
        ////�ȴ��������ã�û�г�������г��뵫��������û����
        //else
        //{
        //    obj = Object.Instantiate(Resources.Load<GameObject>(name));
        //    obj.name = name;
        //}
        #endregion

        #region ���������޺���߼�
        //1.û�г���
        //if(!poolDic.ContainsKey(name))
        //{
        //    obj = Object.Instantiate(Resources.Load<GameObject>(name));
        //    obj.name = name;
        //    ��һ�λ�ȡ��ʱ��ʹ������룬���ڷ����ʱ�򴴽�������
        //    poolDic.Add(name,new PoolData(poolObj,name,obj));
        //}
        //2.��������û�õĶ����ʹ���еĶ��󳬹�����ʱ(����������ж϶�Ӧ��Pop������)
        //else if(poolDic[name].Count > 0 || poolDic[name].UsedCount >= maxNum)
        //{
        //    obj = poolDic[name].Pop();
        //}
        //3.��������û������ʹ���еĶ���Ҳû��������
        //else
        //{
        //    obj = Object.Instantiate(Resources.Load<GameObject>(name));
        //    obj.name = name;
        //    ��¼����ʹ�õĶ���
        //    poolDic[name].PushUsedList(obj);
        //}
        #endregion

        #region ���������޺���߼�(�Ż�)
        //1.û�г��� || 3.��������û������ʹ���еĶ���Ҳû��������
        if(!poolDic.ContainsKey(name) || (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            obj = Object.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;

            if(!poolDic.ContainsKey(name))//��һ�λ�ȡ��ʱ��ʹ������룬���ڷ����ʱ�򴴽�������
                poolDic.Add(name,new PoolData(poolObj,name,obj));
            else//��¼����ʹ�õĶ���
                poolDic[name].PushUsedList(obj);
        }
        //2.��������û�õĶ����ʹ���еĶ��󳬹�����ʱ(����������ж϶�Ӧ��Pop������)
        else
        {
            obj = poolDic[name].Pop();
        }
        #endregion

        return obj;
    }

    /// <summary>
    /// ��������зŶ���ķ���
    /// </summary>
    /// <param name="obj">Ҫ����Ķ���</param>
    public void PushObj(GameObject obj)
    {
        #region ���Ż�
        //obj.SetActive(false);
        ////�������ӹ�ϵ
        //obj.transform.SetParent(poolObj.transform);
        #endregion

        //��ΪҪ��¼������ʹ���еĶ������Գ��벻�����ﴴ����������ȡ����û�оʹ�����������´������벢��¼

        //if(!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name,new PoolData(poolObj,obj.name));

        poolDic[obj.name].Push(obj);

        #region ���Ż�
        ////ֱ�ӷţ���name����
        //if(poolDic.ContainsKey(name))
        //{
        //    poolDic[name].Push(obj);
        //}
        ////�ȴ������룬�ٷţ�û��name����
        //else
        //{
        //    poolDic.Add(name,new Stack<GameObject>());
        //    poolDic[name].Push(obj);
        //}
        #endregion
    }

    /// <summary>
    /// ��ն���ض�������
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
    }
}