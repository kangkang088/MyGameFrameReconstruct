using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test6Mgr : BaseManager<Test6Mgr>
{
    private Coroutine testFunc;

    private Test6Mgr()
    {
    }

    public void ICanUpdate()
    {
        MonoMgr.Instance.AddUpdateListener(MyUpdate);
    }

    public void ICanStopUpdate()
    {
        MonoMgr.Instance.RemoveUpdateListener(MyUpdate);
    }

    public void ICanStartCoroutine()
    {
        testFunc = MonoMgr.Instance.StartCoroutine(Test());
    }

    public void ICanStopCoroutine()
    {
        MonoMgr.Instance.StopCoroutine(testFunc);
    }

    private void MyUpdate()
    {
        Debug.Log("MyUpdate Invoke");
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("3");
    }
}
