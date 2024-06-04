using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test6Mgr : BaseManager<Test6Mgr> {
    private Coroutine coroutine;
    private Test6Mgr() {

    }
    public void ICanUpdate() {
        MonoMgr.Instance.AddUpdateListener(MyUpdate);
    }
    public void ICanStopUpdate() {
        MonoMgr.Instance.RemoveUpdateListener(MyUpdate);
    }
    public void MyUpdate() {
        Debug.Log("Test6Mgr");
    }
    public void ICanCoroutine() {
        coroutine = MonoMgr.Instance.StartCoroutine(Test());
    }
    public void ICanStopCoroutine() {
        MonoMgr.Instance.StopCoroutine(coroutine);
    }
    private IEnumerator Test() {
        yield return new WaitForSeconds(3);
        Debug.Log("Coroutine");
    }
}
