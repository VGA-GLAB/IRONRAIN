using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マルチロックオン時のUIを管理します
/// </summary>
public class MultiLockOn
{
    [SerializeField] private RadarMap _radarMap;
    public List<GameObject> MultiLockEnemies { get; } = new(); //マルチロック時のエネミー 

    public void LockOn(List<GameObject> enemies)
    {
        _radarMap.ResetUI(); //全てのエネミーのロックオンを外す

        if (MultiLockEnemies != null)　MultiLockEnemies.Clear();

        foreach (var enemy in enemies)
        {
            if (!_radarMap._enemyMaps.ContainsKey(enemy))
            {
                continue;
            }
            else
            {
                MultiLockEnemies.Add(enemy);
                AgentScript agentScript = enemy.GetComponent<AgentScript>(); 
                agentScript.IsRockOn = true;
                var enemyUI = _radarMap._enemyMaps[enemy].gameObject.GetComponent<TargetIcon>();
                enemyUI.LockOn();
            }
        }
    }
}
