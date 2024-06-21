using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        TestMgr.Instance.TestLog();
        TestMgr.GetInstance().TestLog();

        Test2Mgr.Instance.TestLog();
        Test2Mgr2.Instance.TestLog();


        ResMgr.Instance.LoadAsync<GameObject>("Test",(obj) =>
        {
            Instantiate(obj);
        });

        ResMgr.Instance.LoadAsync<GameObject>("Test",(obj) =>
        {
            Instantiate(obj);
        });

        Instantiate(ResMgr.Instance.Load<GameObject>("Test"));
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
        #endregion
    }
}