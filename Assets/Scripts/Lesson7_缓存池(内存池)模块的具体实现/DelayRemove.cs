using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayRemove : MonoBehaviour
{
    public string poolName;
    void OnEnable()
    {
        Invoke("RemoveMe",1f);
    }
    private void RemoveMe() {
        PoolMgr.Instance.PushGameObject(this.gameObject);
    }
}
