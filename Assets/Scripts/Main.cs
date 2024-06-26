using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public RawImage rawImage;

    // Start is called before the first frame update
    private void Start()
    {
        TestMgr.Instance.TestLog();
        TestMgr.GetInstance().TestLog();

        Test2Mgr.Instance.TestLog();
        Test2Mgr2.Instance.TestLog();

        ResMgr.Instance.LoadAsync<GameObject>("Test",TestFunc);
        Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        ResMgr.Instance.LoadAsync<GameObject>("Test",TestFunc);
        Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        ResMgr.Instance.UnloadAsset<GameObject>("Test",true,TestFunc);

        Instantiate(ResMgr.Instance.Load<GameObject>("Test"));
        Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        //Instantiate(EditorResMgr.Instance.LoadEditorRes<GameObject>("Cube"));

        //Coroutine coroutine = StartCoroutine(TestFunc());
        //StopCoroutine(coroutine);

        //AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/PC/test");

        //ABMgr.Instance.LoadResAsync<GameObject>("test","Cube",(obj) =>
        //{
        //    Instantiate(obj).name = "ABMgrCreateObj";
        //});

        //ABMgr.Instance.LoadResAsync<GameObject>("test","Cube",(obj) =>
        //{
        //    Instantiate(obj).name = "ABMgrCreateObj";
        //},true);

        //ABMgr.Instance.LoadResAsync<GameObject>("test","Cube",(obj) =>
        //{
        //    Instantiate(obj).name = "ABMgrCreateObj";
        //});

        //UWQResMgr.Instance.LoadRes<byte[]>("file://" + Application.streamingAssetsPath + "/test.txt",(bytes) =>
        //{
        //    Debug.Log("File Content Length:" + bytes.Length);
        //},() =>
        //{
        //    Debug.Log("Failure");
        //});

        //UWQResMgr.Instance.LoadRes<Texture>("file://" + Application.streamingAssetsPath + "/1.jpeg",(texture) =>
        //{
        //    rawImage.texture = texture;
        //},() =>
        //{
        //    Debug.Log("Failure");
        //});

        //UWQResMgr.Instance.LoadRes<AssetBundle>("file://" + Application.streamingAssetsPath + "/test",(ab) =>
        //{
        //    Debug.Log("AB Name:" + ab.name);
        //},() =>
        //{
        //    Debug.Log("Failure");
        //});

        //ABResMgr.Instance.LoadResAsync<GameObject>("test","Cube",(res) =>
        //{
        //    Instantiate(res).name = "ABResMgr Create(Editor Mode)";
        //});

        UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Middle,(panel) =>
        {
            print("The first show.");
        });

        UIMgr.Instance.HidePanel<BeginPanel>();

        UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Middle,(panel) =>
        {
            print("The second show.");
        });

        UIMgr.Instance.GetPabnel<BeginPanel>((panel) =>
        {
            print("Get Panel.");
        });

        InputMgr.Instance.OpenOrCloseInputManager(true);

        InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Skill1,KeyCode.J,InputInfo.E_InputType.Down);
        InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Skill2,KeyCode.K,InputInfo.E_InputType.Up);
        InputMgr.Instance.ChangeMouseInfo(E_EventType.E_Input_Skill3,0,InputInfo.E_InputType.Down);

        EventCenter.Instance.AddListener(E_EventType.E_Input_Skill1,Skill1);

        EventCenter.Instance.AddListener(E_EventType.E_Input_Skill2,Skill2);

        EventCenter.Instance.AddListener(E_EventType.E_Input_Skill3,Skill3);

        EventCenter.Instance.AddListener<float>(E_EventType.E_InputHotKey_Horizontal,CheckHorizontalHotKey);
    }

    private void Skill3()
    {
        print("Skill3");
    }

    private void Skill2()
    {
        print("Skill2");
    }

    private void Skill1()
    {
        print("Skill1");
    }

    private void CheckHorizontalHotKey(float value)
    {
        print(value);
    }

    private IEnumerator TestFunc()
    {
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/PC/test");
        yield return req;
        print(req.assetBundle.name);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Test6Mgr.Instance.ICanUpdate();
            Test6Mgr.Instance.ICanStartCoroutine();
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            Test6Mgr.Instance.ICanStopUpdate();
            Test6Mgr.Instance.ICanStopCoroutine();
        }

        #region ª∫¥Ê≥ÿ≤‚ ‘

        if(Input.GetMouseButtonDown(0))
        {
            PoolMgr.Instance.GetObj("Test/Cube");
        }
        if(Input.GetMouseButtonDown(1))
        {
            PoolMgr.Instance.GetObj("Test/Sphere");
        }

        #endregion ª∫¥Ê≥ÿ≤‚ ‘

        if(Input.GetKeyDown(KeyCode.S))
        {
            UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Middle,(panel) =>
            {
                print("S");
            });
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            UIMgr.Instance.HidePanel<BeginPanel>(true);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            InputMgr.Instance.GetInputInfo((inputInfo) =>
            {
                if(inputInfo.keyboardOrMouse == InputInfo.E_KeyboardOrMouse.Keyboard)
                {
                    print(inputInfo.keyCode);
                }
            });
        }

        if(Input.GetKeyDown(KeyCode.V)) 
        {
            timerID = TimerMgr.Instance.CreateTimer(false, 5000,() =>
            {
                print("Timer Over.");
            },1000,() =>
            {
                print("Interval");
            });
        }
    }
    private int timerID;


    private void TestFunc(GameObject obj)
    {
        Instantiate(obj);
    }

    private float value;
    private float oldValue;

    private AudioSource source;

    private void OnGUI()
    {
        if(GUILayout.Button("PlayBegin"))
            MusicMgr.Instance.PlayBKMusic("Begin");
        if(GUILayout.Button("PlayFight"))
            MusicMgr.Instance.PlayBKMusic("Fight");
        if(GUILayout.Button("StopMusic"))
            MusicMgr.Instance.StopBKMusic();
        if(GUILayout.Button("PauseMusic"))
            MusicMgr.Instance.PauseBKMusic();

        value = GUILayout.HorizontalSlider(value,0,1);
        if(oldValue != value)
        {
            oldValue = value;
            MusicMgr.Instance.ChangeBKMusicVaule(value);
            MusicMgr.Instance.ChangeSoundValue(value);
        }

        if(GUILayout.Button("PlayFight1"))
            MusicMgr.Instance.PlaySound("Fight1",true,false,(audioSource) =>
            {
                source = audioSource;
            });
        if(GUILayout.Button("StopSound"))
        {
            MusicMgr.Instance.StopSound(source);
            source = null;
        }
    }
}