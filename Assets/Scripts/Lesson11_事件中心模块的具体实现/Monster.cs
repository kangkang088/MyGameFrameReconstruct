using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public string monsterName = "kangkang";
    public int monsterID = 1;
    private void Start() {
        //Dead();
    }
    public void Dead() {
        Debug.Log("����������");
        EventCenter.Instance.EventTrigger<Monster>(E_EventType.E_Monster_Dead,this);

        #region �����д��
        //GameObject.Find("Task").GetComponent<Task>().TaskWaitMonsterDeadDo();
        //GameObject.Find("Player").GetComponent<Player>().PlayerWaitMonsterDeadDo();
        //GameObject.Find("Other").GetComponent<Other>().OtherWaitMonsterDeadDo();
        #endregion
    }
}
