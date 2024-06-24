using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 存储所有需要使用的控件
    /// </summary>
    protected Dictionary<string,UIBehaviour> controlDic = new();

    /// <summary>
    /// 控件默认名字容器。如果得到的控件名存在于这个容器，说明这个控件名是默认名，往往是一些我们代码不太关心的控件，比如Button下的Text。
    /// </summary>
    private static List<string> defaultNamList = new(new string[] { "Image","Text (TMP)","RawImage","Background","Checkmark","Label","Text (Legacy)","Arrow","Placeholder","Fill","Handle","Viewport","Scrollbar Horizontal","Scrollbar Vertical" });

    protected void Awake()
    {
        //为了避免某一个对象上存在两种控件的情况，应该优先查找重要的组件
        FindChildrenControl<Button>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Dropdown>();
        //得到了重要组件，就算该对象还有其他次要组件，也可以获取
        FindChildrenControl<Text>();
        FindChildrenControl<TextMeshProUGUI>();
        FindChildrenControl<Image>();
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public abstract void ShowMe();

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// 获取指定UI控件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <param name="controlName">控件名</param>
    /// <returns></returns>
    public T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if(controlDic.ContainsKey(controlName))
        {
            T control = controlDic[controlName] as T;
            if(control == null)
                Debug.LogError($"Not exist name:{controlName} type:{typeof(T)} control.");
            return control;
        }
        else
        {
            Debug.LogError($"Not exist name:{controlName} control.");
            return null;
        }
    }

    protected virtual void ClickButton(string buttonName)
    {
    }

    protected virtual void SliderValueChange(string sliderName,float value)
    {
    }

    protected virtual void ToggleValueChange(string toggleName,bool value)
    {
    }

    /// <summary>
    /// 查找控件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = GetComponentsInChildren<T>();
        for(int i = 0;i < controls.Length;i++)
        {
            string controlName = controls[i].gameObject.name;
            if(!controlDic.ContainsKey(controlName))
            {
                if(!defaultNamList.Contains(controlName))
                {
                    controlDic.Add(controlName,controls[i]);

                    //判断控件的类型，决定是否加入事件监听
                    if(controls[i] is Button)
                    {
                        (controls[i] as Button).onClick.AddListener(() =>
                        {
                            ClickButton(controlName);
                        });
                    }
                    else if(controls[i] is Slider)
                    {
                        (controls[i] as Slider).onValueChanged.AddListener((value) =>
                        {
                            SliderValueChange(controlName,value);
                        });
                    }
                    else if(controls[i] is Toggle)
                    {
                        (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                        {
                            ToggleValueChange(controlName,value);
                        });
                    }
                }
            }
        }
    }
}