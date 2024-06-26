using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    // Start is called before the first frame update
    private void Start()
    {
        print(GetControl<Button>("BtnBegin").name);

        UIMgr.AddCustomEventListener(GetControl<Button>("BtnBegin"),UnityEngine.EventSystems.EventTriggerType.PointerEnter,(data) =>
        {
            print("Mouse Enter");
        });

        UIMgr.AddCustomEventListener(GetControl<Button>("BtnBegin"),UnityEngine.EventSystems.EventTriggerType.PointerExit,(data) =>
        {
            print("Mouse Exit");
        });

        EventCenter.Instance.AddListener<float>(E_EventType.E_SceneProgress_Change,ChangeSceneProgress);
    }

    private void ChangeSceneProgress(float progress)
    {
        print("Load Progress:" + progress.ToString() + "/1.");
    }

    protected override void ClickButton(string buttonName)
    {
        switch(buttonName)
        {
            case "BtnBegin":
                SceneMgr.Instance.LoadSceneAsync("Test",() =>
                {
                    print("Load Over.");
                });
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

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveListener<float>(E_EventType.E_SceneProgress_Change,ChangeSceneProgress);
    }
}