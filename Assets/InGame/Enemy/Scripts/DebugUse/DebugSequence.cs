using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    public class DebugSequence : MonoBehaviour
    {
        [SerializeField] private EnemyManager _enemyManager;

        private bool _isEnable;

        private void Update()
        {
            // 有効/無効を切り替え
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                _isEnable = !_isEnable;
            }
        }

        private void OnGUI()
        {
            if (!_isEnable) return;

            const float x = 0;
            const float y = 33;
            const float w = 300;
            const float h = 70;

            if (GUI.Button(new Rect(x, y, w, h), "AttackSeq"))
            {
                // 現状、唯一の視界で検知するタイプなので特になし。
            }
            else if (GUI.Button(new Rect(x, y + h, w, h), "TouchPanelSeq"))
            {
                _enemyManager.DetectPlayer(EnemyManager.Sequence.TouchPanel);
            }
            else if (GUI.Button(new Rect(x, y + 2 * h, w, h), "QTETutorialSeq"))
            {
                _enemyManager.DetectPlayer(EnemyManager.Sequence.QTETutorial);
                _enemyManager.Pause(EnemyManager.Sequence.TouchPanel);
            }
            else if (GUI.Button(new Rect(x, y + 3 * h, w, h), "MultiBattleSeq"))
            {
                _enemyManager.DetectPlayer(EnemyManager.Sequence.MultiBattle);
                _enemyManager.PlayNpcEvent(EnemyManager.Sequence.MultiBattle);
            }
            else if (GUI.Button(new Rect(x, y + 4 * h, w, h), "BossStartSeq"))
            {
                _enemyManager.BossStart();
            }
            else if (GUI.Button(new Rect(x, y + 5 * h, w, h), "FirstFunnel"))
            {
                _enemyManager.FunnelExpand();
            }
            else if (GUI.Button(new Rect(x, y + 6 * h, w, h), "SecondFunnel"))
            {
                _enemyManager.FunnelExpand();
            }
            else if (GUI.Button(new Rect(x, y + 7 * h, w, h), "BreakLeftArm"))
            {
                _enemyManager.BreakLeftArm();
            }
            else if (GUI.Button(new Rect(x, y + 8 * h, w, h), "FirstBossQTE"))
            {
                _enemyManager.BossFirstQte();
            }
            else if (GUI.Button(new Rect(x, y + 9 * h, w, h), "SecondQTE"))
            {
                _enemyManager.BossSecondQte();
            }
        }
    }
}
