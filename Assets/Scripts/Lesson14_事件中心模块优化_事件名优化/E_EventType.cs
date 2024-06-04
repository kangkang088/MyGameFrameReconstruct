using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型枚举
/// </summary>
public enum E_EventType
{
    /// <summary>
    /// 怪物死亡事件 - 参数：Monster
    /// </summary>
    E_Monster_Dead,
    /// <summary>
    /// 玩家奖励事件 - 参数：int
    /// </summary>
    E_Player_GetReward,
    /// <summary>
    /// 测试事件 - 参数：无
    /// </summary>
    E_Test
}
