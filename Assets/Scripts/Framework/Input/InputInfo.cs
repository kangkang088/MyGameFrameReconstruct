using UnityEngine;

/// <summary>
/// ������Ϣ��
/// </summary>
public class InputInfo
{
    public enum E_KeyboardOrMouse
    {
        Keyboard, Mouse
    }

    public enum E_InputType
    {
        Down, Up, Always
    }

    public E_KeyboardOrMouse keyboardOrMouse;
    public E_InputType inputType;

    public KeyCode keyCode;
    public int mouseID;

    /// <summary>
    /// �������빹�캯��
    /// </summary>
    /// <param name="inputType">��������</param>
    /// <param name="keyCode">��ֵ</param>
    public InputInfo(E_InputType inputType,KeyCode keyCode)
    {
        keyboardOrMouse = E_KeyboardOrMouse.Keyboard;
        this.inputType = inputType;
        this.keyCode = keyCode;
    }

    /// <summary>
    /// ������빹�캯��
    /// </summary>
    /// <param name="inputType">��������</param>
    /// <param name="mouseID">�������</param>
    public InputInfo(E_InputType inputType,int mouseID)
    {
        keyboardOrMouse = E_KeyboardOrMouse.Mouse;
        this.inputType = inputType;
        this.mouseID = mouseID;
    }
}