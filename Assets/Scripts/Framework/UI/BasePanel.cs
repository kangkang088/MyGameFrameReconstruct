using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// �洢������Ҫʹ�õĿؼ�
    /// </summary>
    protected Dictionary<string,UIBehaviour> controlDic = new();

    /// <summary>
    /// �ؼ�Ĭ����������������õ��Ŀؼ������������������˵������ؼ�����Ĭ������������һЩ���Ǵ��벻̫���ĵĿؼ�������Button�µ�Text��
    /// </summary>
    private static List<string> defaultNamList = new(new string[] { "Image","Text (TMP)","RawImage","Background","Checkmark","Label","Text (Legacy)","Arrow","Placeholder","Fill","Handle","Viewport","Scrollbar Horizontal","Scrollbar Vertical" });

    protected void Awake()
    {
        //Ϊ�˱���ĳһ�������ϴ������ֿؼ��������Ӧ�����Ȳ�����Ҫ�����
        FindChildrenControl<Button>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Dropdown>();
        //�õ�����Ҫ���������ö�����������Ҫ�����Ҳ���Ի�ȡ
        FindChildrenControl<Text>();
        FindChildrenControl<TextMeshProUGUI>();
        FindChildrenControl<Image>();
    }

    /// <summary>
    /// ��ʾ���
    /// </summary>
    public abstract void ShowMe();

    /// <summary>
    /// �������
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// ��ȡָ��UI�ؼ�
    /// </summary>
    /// <typeparam name="T">�ؼ�����</typeparam>
    /// <param name="controlName">�ؼ���</param>
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
    /// ���ҿؼ�
    /// </summary>
    /// <typeparam name="T">�ؼ�����</typeparam>
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

                    //�жϿؼ������ͣ������Ƿ�����¼�����
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