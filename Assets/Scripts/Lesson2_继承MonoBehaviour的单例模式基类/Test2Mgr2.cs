using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2Mgr2 : SingletonAutoMono<Test2Mgr2>
{
    private int i = 100;
    public void TestLog() {
        print("Test2Mgr2");
        print(i);
    }
}
