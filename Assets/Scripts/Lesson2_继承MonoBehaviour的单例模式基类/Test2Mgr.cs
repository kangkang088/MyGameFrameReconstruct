using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Test2Mgr : SingletonMono<Test2Mgr> {
    private int i;
    public void TestLog() {
        print("Test2Mgr");
        print(i);
    }
    protected override void Awake() {
        //��дAwakeʱ������ʡ����һ��
        base.Awake();
        i = 10;
    }
}
