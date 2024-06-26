using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 输入控制模块管理器
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    private Dictionary<E_EventType,InputInfo> inputDic = new();

    private InputInfo nowInputInfo;

    //用于改键时获取输入信息的委托
    private UnityAction<InputInfo> getInputInfoCallback;

    //是否开启检测获取输入信息
    private bool isBeginCheckInputInfo;

    //是否开启输入检测
    private bool isOpen;

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }

    /// <summary>
    /// 提供给外部改键并进行初始化的方法
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="keyCode">键值</param>
    /// <param name="inputType">键行为类型</param>
    public void ChangeKeyboardInfo(E_EventType eventType,KeyCode keyCode,InputInfo.E_InputType inputType)
    {
        //初始化
        if(!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,keyCode));
        }
        //改键
        else
        {
            inputDic[eventType].keyboardOrMouse = InputInfo.E_KeyboardOrMouse.Keyboard;
            inputDic[eventType].keyCode = keyCode;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// 提供给外部改鼠标并进行初始化的方法
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="mouseID">鼠标索引</param>
    /// <param name="inputType">鼠标行为类型</param>
    public void ChangeMouseInfo(E_EventType eventType,int mouseID,InputInfo.E_InputType inputType)
    {
        //初始化
        if(!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,mouseID));
        }
        //改键
        else
        {
            inputDic[eventType].keyboardOrMouse = InputInfo.E_KeyboardOrMouse.Mouse;
            inputDic[eventType].mouseID = mouseID;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// 移除指定行为的输入监听
    /// </summary>
    /// <param name="eventType">事件类型</param>
    public void RemoveInputInfo(E_EventType eventType)
    {
        if(inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
    }

    /// <summary>
    /// 开启或者关闭输入管理模块
    /// </summary>
    /// <param name="isOpen">是否开启</param>
    public void OpenOrCloseInputManager(bool isOpen)
    {
        this.isOpen = isOpen;
    }

    /// <summary>
    /// 获取输入键位的信息
    /// </summary>
    /// <param name="callback"></param>
    public void GetInputInfo(UnityAction<InputInfo> callback)
    {
        getInputInfoCallback = callback;
        MonoMgr.Instance.StartCoroutine(BeginCheckInputInfo());
    }

    private IEnumerator BeginCheckInputInfo()
    {
        //等一帧
        yield return 0;
        //一帧后变为true
        isBeginCheckInputInfo = true;
    }

    /// <summary>
    /// 检测键盘和鼠标按键输入
    /// </summary>
    private void InputUpdate()
    {
        if(isBeginCheckInputInfo)
        {
            //当一个键按下时，遍历所有按键信息，确认是哪个键按下
            if(Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                //遍历监听所有键位的按下，来得到对应的输入信息
                //键盘
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach(KeyCode inputKeyCode in keyCodes)
                {
                    //到底是谁按下了
                    if(Input.GetKeyDown(inputKeyCode))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down,inputKeyCode);
                        break;
                    }
                }
                //鼠标
                for(int i = 0;i < 3;i++)
                {
                    if(Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down,i);
                        break;
                    }
                }
                //传递给外部
                getInputInfoCallback.Invoke(inputInfo);
                getInputInfoCallback = null;
                //检测一次后停止检测
                isBeginCheckInputInfo = false;
            }
        }

        if(!isOpen)
            return;

        foreach(E_EventType eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            //键盘输入检测
            if(nowInputInfo.keyboardOrMouse == InputInfo.E_KeyboardOrMouse.Keyboard)
            {
                switch(nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if(Input.GetKeyDown(nowInputInfo.keyCode))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;

                    case InputInfo.E_InputType.Up:
                        if(Input.GetKeyUp(nowInputInfo.keyCode))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;

                    case InputInfo.E_InputType.Always:
                        if(Input.GetKey(nowInputInfo.keyCode))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                }
            }
            //鼠标输入检测
            else
            {
                switch(nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if(Input.GetMouseButtonDown(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;

                    case InputInfo.E_InputType.Up:
                        if(Input.GetMouseButtonUp(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;

                    case InputInfo.E_InputType.Always:
                        if(Input.GetMouseButton(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                }
            }
        }

        EventCenter.Instance.EventTrigger(E_EventType.E_InputHotKey_Horizontal,Input.GetAxis("Horizontal"));
        EventCenter.Instance.EventTrigger(E_EventType.E_InputHotKey_Vertical,Input.GetAxis("Vertical"));
    }
}