using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestMgr.Instance.TestLog();
        Test2Mgr.Instance.TestLog();
        Test2Mgr2.Instance.TestLog();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Test6Mgr.Instance.ICanUpdate();
            Test6Mgr.Instance.ICanCoroutine();
        }
        if(Input.GetKeyUp(KeyCode.Space)) {
            Test6Mgr.Instance.ICanStopUpdate();
            Test6Mgr.Instance.ICanStopCoroutine();
        }

        #region ª∫¥Ê≥ÿ≤‚ ‘
        if(Input.GetMouseButtonDown(0)) {
            PoolMgr.Instance.GetGameObject("Test/Cube");
        }
        if(Input.GetMouseButtonDown(1)) {
            PoolMgr.Instance.GetGameObject("Test/Sphere");
        }
        #endregion

        if(Input.GetKeyDown(KeyCode.X)) {
            GameObject.Find("Monster").GetComponent<Monster>().Dead();

            EventCenter.Instance.EventTrigger(E_EventType.E_Test);
        }
    }
}
