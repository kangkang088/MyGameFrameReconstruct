using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    // Start is called before the first frame update
    private void Start()
    {
        //foreach(string name in controlDic.Keys)
        //{
        //    Debug.Log(name);
        //}

        print(GetControl<Button>("BtnBegin").name);
    }

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "BtnBegin":
                print("Begin Click");
                break;
            case "BtnSetting":
                print("Setting Click");
                break;
            case "BtnQuit":
                print("Quit Click");
                break;
        }
    }

    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        
    }
}