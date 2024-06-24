using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UILayer
{
    Bottom, Middle, Top, System
}

/// <summary>
/// UI管理器，管理所有面板  规则：面板预设体和面板类名一致
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

    //层级父对象
    private Transform bottomLayer;

    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    //存储所有面板的容器
    private Dictionary<string,BasePanelInfo> panelDic = new();

    private UIManager()
    {
        //动态创建UI摄像机
        uiCamera = Object.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        Object.DontDestroyOnLoad(uiCamera.gameObject);

        //动态创建Canvas
        uiCanvas = Object.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        //设置UI摄像机
        uiCanvas.worldCamera = uiCamera;
        //找到层级父对象
        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");
        Object.DontDestroyOnLoad(uiCanvas.gameObject);

        //动态创建EventSystem
        uiEventSystem = Object.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        Object.DontDestroyOnLoad(uiEventSystem.gameObject);
    }

    /// <summary>
    /// 获取层级父对象Transform
    /// </summary>
    /// <param name="layer">层级枚举</param>
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
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="layer">面板层级</param>
    /// <param name="callback">异步加载面板完成后的回调</param>
    /// <param name="isSync">是否同步</param>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle,UnityAction<T> callback = null,bool isSync = false) where T : BasePanel
    {
        //规则：预设体名和面板类名一致
        string panelName = typeof(T).Name;

        //存在面板
        if(panelDic.ContainsKey(panelName))
        {
            //取出字典中的展位数据，进行判断
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //正在异步加载中
            if(panelInfo.panel == null)
            {
                //显示了又隐藏，然后又显示，直接设为false
                panelInfo.isHide = false;

                //等待加载完毕：只需要记录回调，加载完毕后调用
                if(callback != null)
                    panelInfo.callback += callback;
            }
            //异步加载结束
            else
            {
                //如果是失活状态，直接激活面板就可以了
                if(!panelInfo.panel.gameObject.activeSelf)
                    panelInfo.panel.gameObject.SetActive(true);

                panelInfo.panel.ShowMe();
                callback?.Invoke(panelInfo.panel);
            }
            return;
        }

        //不存在面板，先存入字典占位，之后异步又要显示，才可以根据这个占位的信息来判断是否加载完毕
        panelDic.Add(panelName,new PanelInfo<T>(callback));

        ABResMgr.Instance.LoadResAsync<GameObject>("ui",panelName,(res) =>
        {
            //取出字典中的展位数据，进行判断
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //异步加载结束前，就想要隐藏了。
            if(panelInfo.isHide)
            {
                panelDic.Remove(panelName);
                return;
            }

            Transform father = GetLayer(layer) ?? middleLayer;
            GameObject panelObj = Object.Instantiate(res,father,false);

            T panel = panelObj.GetComponent<T>();
            panel.ShowMe();
            //传出去使用
            panelInfo.callback?.Invoke(panel);
            //回调执行完毕后置空
            panelInfo.callback = null;
            //存储panel
            panelInfo.panel = panel;
        },isSync);
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if(panelDic.ContainsKey(panelName))
        {
            //取出字典中的展位数据，进行判断
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //存在，但是正在加载中
            if(panelInfo.panel == null)
            {
                //还没加载完，但这个面板要隐藏了。也不要执行回调了，给你置空
                panelInfo.isHide = true;
                panelInfo.callback = null;
            }
            //已经加载结束
            else
            {
                panelInfo.panel.HideMe();

                if(isDestroy)
                {
                    //销毁面板
                    Object.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                }
                else
                    //不销毁，只是失活，下次再显示就直接复用
                    panelInfo.panel.gameObject.SetActive(false);
                
            }
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>面板类</returns>
    public void GetPabnel<T>(UnityAction<T> callback) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if(panelDic.ContainsKey(panelName))
        {
            //取出字典中的展位数据，进行判断
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //正在加载中
            if(panelInfo.panel == null)
            {
                //记录回调，加载结束执行回调，把面板传递给外部
                panelInfo.callback += callback;
            }
            //加载完毕,并且不隐藏
            else if(!panelInfo.isHide)
            {
                //传出去
                callback?.Invoke(panelInfo.panel);
            }
        }
    }
}