using UnityEngine;

public class Test2Mgr2 : SingletonAutoMono<Test2Mgr2>
{
    private int i;

    protected void Awake()
    {
        i = 10;
    }

    public void TestLog()
    {
        Debug.Log("Test2Mgr2" + i);
    }
}