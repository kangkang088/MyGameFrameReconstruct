using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMgr:BaseManager<TestMgr> {
    private List<object> list = new List<object>();
    private TestMgr() {
        
    }
    public void TestLog() {
        Debug.Log("TestMgr");
    }

    public void AddData(object data) {
        lock(lockObj) {
            list.Add(data);
        }
    }
}
