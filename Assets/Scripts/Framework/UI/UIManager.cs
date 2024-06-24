using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UILayer
{
    Bottom, Middle, Top, System
}

/// <summary>
/// UI�������������������  �������Ԥ������������һ��
/// </summary>
public class UIManager : BaseManager<UIManager>
{
    private abstract class BasePanelInfo
    {
    }

    private class PanelInfo<T> : BasePanelInfo where T : BasePanel
    {
        public T panel;
        public UnityAction<T> callback;
        public bool isHide;

        public PanelInfo(UnityAction<T> callback)
        {
            this.callback += callback;
        }
    }

    private Camera uiCamera;
    private Canvas uiCanvas;
    private EventSystem uiEventSystem;

    //�㼶������
    private Transform bottomLayer;

    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    //�洢������������
    private Dictionary<string,BasePanelInfo> panelDic = new();

    private UIManager()
    {
        //��̬����UI�����
        uiCamera = Object.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        Object.DontDestroyOnLoad(uiCamera.gameObject);

        //��̬����Canvas
        uiCanvas = Object.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        //����UI�����
        uiCanvas.worldCamera = uiCamera;
        //�ҵ��㼶������
        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");
        Object.DontDestroyOnLoad(uiCanvas.gameObject);

        //��̬����EventSystem
        uiEventSystem = Object.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        Object.DontDestroyOnLoad(uiEventSystem.gameObject);
    }

    /// <summary>
    /// ��ȡ�㼶������Transform
    /// </summary>
    /// <param name="layer">�㼶ö��</param>
    /// <returns></returns>
    public Transform GetLayer(E_UILayer layer)
    {
        switch(layer)
        {
            case E_UILayer.Bottom:
                return bottomLayer;

            case E_UILayer.Middle:
                return middleLayer;

            case E_UILayer.Top:
                return topLayer;

            case E_UILayer.System:
                return systemLayer;

            default:
                return null;
        }
    }

    /// <summary>
    /// ��ʾ���
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <param name="layer">���㼶</param>
    /// <param name="callback">�첽���������ɺ�Ļص�</param>
    /// <param name="isSync">�Ƿ�ͬ��</param>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle,UnityAction<T> callback = null,bool isSync = false) where T : BasePanel
    {
        //����Ԥ���������������һ��
        string panelName = typeof(T).Name;

        //�������
        if(panelDic.ContainsKey(panelName))
        {
            //ȡ���ֵ��е�չλ���ݣ������ж�
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //�����첽������
            if(panelInfo.panel == null)
            {
                //��ʾ�������أ�Ȼ������ʾ��ֱ����Ϊfalse
                panelInfo.isHide = false;

                //�ȴ�������ϣ�ֻ��Ҫ��¼�ص���������Ϻ����
                if(callback != null)
                    panelInfo.callback += callback;
            }
            //�첽���ؽ���
            else
            {
                //�����ʧ��״̬��ֱ�Ӽ������Ϳ�����
                if(!panelInfo.panel.gameObject.activeSelf)
                    panelInfo.panel.gameObject.SetActive(true);

                panelInfo.panel.ShowMe();
                callback?.Invoke(panelInfo.panel);
            }
            return;
        }

        //��������壬�ȴ����ֵ�ռλ��֮���첽��Ҫ��ʾ���ſ��Ը������ռλ����Ϣ���ж��Ƿ�������
        panelDic.Add(panelName,new PanelInfo<T>(callback));

        ABResMgr.Instance.LoadResAsync<GameObject>("ui",panelName,(res) =>
        {
            //ȡ���ֵ��е�չλ���ݣ������ж�
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //�첽���ؽ���ǰ������Ҫ�����ˡ�
            if(panelInfo.isHide)
            {
                panelDic.Remove(panelName);
                return;
            }

            Transform father = GetLayer(layer) ?? middleLayer;
            GameObject panelObj = Object.Instantiate(res,father,false);

            T panel = panelObj.GetComponent<T>();
            panel.ShowMe();
            //����ȥʹ��
            panelInfo.callback?.Invoke(panel);
            //�ص�ִ����Ϻ��ÿ�
            panelInfo.callback = null;
            //�洢panel
            panelInfo.panel = panel;
        },isSync);
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if(panelDic.ContainsKey(panelName))
        {
            //ȡ���ֵ��е�չλ���ݣ������ж�
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //���ڣ��������ڼ�����
            if(panelInfo.panel == null)
            {
                //��û�����꣬��������Ҫ�����ˡ�Ҳ��Ҫִ�лص��ˣ������ÿ�
                panelInfo.isHide = true;
                panelInfo.callback = null;
            }
            //�Ѿ����ؽ���
            else
            {
                panelInfo.panel.HideMe();

                if(isDestroy)
                {
                    //�������
                    Object.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                }
                else
                    //�����٣�ֻ��ʧ��´�����ʾ��ֱ�Ӹ���
                    panelInfo.panel.gameObject.SetActive(false);
                
            }
        }
    }

    /// <summary>
    /// ��ȡ���
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <returns>�����</returns>
    public void GetPabnel<T>(UnityAction<T> callback) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if(panelDic.ContainsKey(panelName))
        {
            //ȡ���ֵ��е�չλ���ݣ������ж�
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //���ڼ�����
            if(panelInfo.panel == null)
            {
                //��¼�ص������ؽ���ִ�лص�������崫�ݸ��ⲿ
                panelInfo.callback += callback;
            }
            //�������,���Ҳ�����
            else if(!panelInfo.isHide)
            {
                //����ȥ
                callback?.Invoke(panelInfo.panel);
            }
        }
    }
}