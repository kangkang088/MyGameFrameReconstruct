using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �������ģ�������
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    private Dictionary<E_EventType,InputInfo> inputDic = new();

    private InputInfo nowInputInfo;

    //���ڸļ�ʱ��ȡ������Ϣ��ί��
    private UnityAction<InputInfo> getInputInfoCallback;

    //�Ƿ�������ȡ������Ϣ
    private bool isBeginCheckInputInfo;

    //�Ƿ���������
    private bool isOpen;

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }

    /// <summary>
    /// �ṩ���ⲿ�ļ������г�ʼ���ķ���
    /// </summary>
    /// <param name="eventType">�¼�����</param>
    /// <param name="keyCode">��ֵ</param>
    /// <param name="inputType">����Ϊ����</param>
    public void ChangeKeyboardInfo(E_EventType eventType,KeyCode keyCode,InputInfo.E_InputType inputType)
    {
        //��ʼ��
        if(!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,keyCode));
        }
        //�ļ�
        else
        {
            inputDic[eventType].keyboardOrMouse = InputInfo.E_KeyboardOrMouse.Keyboard;
            inputDic[eventType].keyCode = keyCode;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// �ṩ���ⲿ����겢���г�ʼ���ķ���
    /// </summary>
    /// <param name="eventType">�¼�����</param>
    /// <param name="mouseID">�������</param>
    /// <param name="inputType">�����Ϊ����</param>
    public void ChangeMouseInfo(E_EventType eventType,int mouseID,InputInfo.E_InputType inputType)
    {
        //��ʼ��
        if(!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,mouseID));
        }
        //�ļ�
        else
        {
            inputDic[eventType].keyboardOrMouse = InputInfo.E_KeyboardOrMouse.Mouse;
            inputDic[eventType].mouseID = mouseID;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// �Ƴ�ָ����Ϊ���������
    /// </summary>
    /// <param name="eventType">�¼�����</param>
    public void RemoveInputInfo(E_EventType eventType)
    {
        if(inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
    }

    /// <summary>
    /// �������߹ر��������ģ��
    /// </summary>
    /// <param name="isOpen">�Ƿ���</param>
    public void OpenOrCloseInputManager(bool isOpen)
    {
        this.isOpen = isOpen;
    }

    /// <summary>
    /// ��ȡ�����λ����Ϣ
    /// </summary>
    /// <param name="callback"></param>
    public void GetInputInfo(UnityAction<InputInfo> callback)
    {
        getInputInfoCallback = callback;
        MonoMgr.Instance.StartCoroutine(BeginCheckInputInfo());
    }

    private IEnumerator BeginCheckInputInfo()
    {
        //��һ֡
        yield return 0;
        //һ֡���Ϊtrue
        isBeginCheckInputInfo = true;
    }

    /// <summary>
    /// �����̺���갴������
    /// </summary>
    private void InputUpdate()
    {
        if(isBeginCheckInputInfo)
        {
            //��һ��������ʱ���������а�����Ϣ��ȷ�����ĸ�������
            if(Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                //�����������м�λ�İ��£����õ���Ӧ��������Ϣ
                //����
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach(KeyCode inputKeyCode in keyCodes)
                {
                    //������˭������
                    if(Input.GetKeyDown(inputKeyCode))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down,inputKeyCode);
                        break;
                    }
                }
                //���
                for(int i = 0;i < 3;i++)
                {
                    if(Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down,i);
                        break;
                    }
                }
                //���ݸ��ⲿ
                getInputInfoCallback.Invoke(inputInfo);
                getInputInfoCallback = null;
                //���һ�κ�ֹͣ���
                isBeginCheckInputInfo = false;
            }
        }

        if(!isOpen)
            return;

        foreach(E_EventType eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            //����������
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
            //���������
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