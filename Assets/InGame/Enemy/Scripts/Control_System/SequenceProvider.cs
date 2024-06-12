using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// EnemyManagerやNpcManagerの処理を呼び出す仲介役。
    /// </summary>
    public class SequenceProvider : MonoBehaviour
    {
        [SerializeField] private NpcManager _npcManager;
        [SerializeField] private EnemyManager _enemyManager;

        public void DetectPlayerOnTutorialSeq() => _enemyManager.DetectPlayer(EnemyManager.Sequence.Tutorial);
        public bool IsAllDefeatedOnTutorialSeq() => _enemyManager.IsAllDefeated(EnemyManager.Sequence.Tutorial);
        public void DetectPlayerOnMultiBattleSeq() => _enemyManager.DetectPlayer(EnemyManager.Sequence.MultiBattle);
        public bool IsAllDefeatedOnMultiBattleSeq() => _enemyManager.IsAllDefeated(EnemyManager.Sequence.MultiBattle);
        public void AppearNPCsOnMultiBattleSeq() => _npcManager.PlayEvent(NpcManager.Sequence.MultiBattle);
        public void BossStart() => _enemyManager.BossStart();
    }

}