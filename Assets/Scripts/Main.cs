using System.Collections;
using System.Collections.Generic;
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
    }
}