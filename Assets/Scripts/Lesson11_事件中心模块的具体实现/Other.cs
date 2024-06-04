using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Other : MonoBehaviour
{
    private void Awake() {
        EventCenter.Instance.AddEventListener<Monster>(E_EventType.E_Monster_Dead,OtherWaitMonsterDeadDo);
    }
    public void OtherWaitMonsterDeadDo(Monster obj) {
        Debug.Log("������ش���" + obj.monsterID);
    }
    private void OnDestroy() {
        EventCenter.Instance.RemoveEventListener<Monster>(E_EventType.E_Monster_Dead,OtherWaitMonsterDeadDo);
    }
}
