using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayRemove : MonoBehaviour
{
    public new string name;

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("RemoveSelf",1f);
    }

    private void RemoveSelf()
    {
        PoolMgr.Instance.PushObj(gameObject);
    }
}
