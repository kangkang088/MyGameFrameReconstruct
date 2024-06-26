using UnityEngine;

/// <summary>
/// 输入信息类
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
    /// 键盘输入构造函数
    /// </summary>
    /// <param name="inputType">输入类型</param>
    /// <param name="keyCode">键值</param>
    public InputInfo(E_InputType inputType,KeyCode keyCode)
    {
        keyboardOrMouse = E_KeyboardOrMouse.Keyboard;
        this.inputType = inputType;
        this.keyCode = keyCode;
    }

    /// <summary>
    /// 鼠标输入构造函数
    /// </summary>
    /// <param name="inputType">输入类型</param>
    /// <param name="mouseID">鼠标索引</param>
    public InputInfo(E_InputType inputType,int mouseID)
    {
        keyboardOrMouse = E_KeyboardOrMouse.Mouse;
        this.inputType = inputType;
        this.mouseID = mouseID;
    }
}