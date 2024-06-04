using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson6 : MonoBehaviour
{
    private void Start() {
        MonoMgr.Instance.AddUpdateListener(MyUpdate);
    }
    private void MyUpdate() {
        print("Lesson6");
    }
}
