using Cysharp.Threading.Tasks;
using Enemy.Funnel;
using UnityEngine;

namespace Enemy.DebugUse
{
    public class DebugCommand : MonoBehaviour
    {
        [Header("ボタンの設定")]
        [SerializeField] private float _buttonX = 0;
        [SerializeField] private float _buttonY = 33;
        [SerializeField] private float _buttonWidth = 300;
        [SerializeField] private float _buttonHeight = 70;

        private EnemyManager _enemyManager;
        
        // コマンド入力可能状態
        private bool _isEnable;

        private void Awake()
        {
            _enemyManager = GetComponent<EnemyManager>();
        }

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
            if (_isEnable) EventRunButton(_buttonX, _buttonY, _buttonWidth, _buttonHeight);
        }

        // イベント実行のボタン
        private void EventRunButton(float x, float y, float w, float h)
        {
            if (EventRunButton(0, "AvoidanceSeq"))
            {
                _enemyManager.DetectPlayer(EnemyManager.Sequence.Avoidance);
            }
            else if (EventRunButton(1, "TouchPanelSeq"))
            {
                _enemyManager.DetectPlayer(EnemyManager.Sequence.TouchPanel);
            }
            else if (EventRunButton(2, "QTETutorialSeq"))
            {
                _enemyManager.DetectPlayer(EnemyManager.Sequence.QTETutorial);
            }
            else if (EventRunButton(3, "MultiBattleSeq"))
            {
                _enemyManager.DetectPlayer(EnemyManager.Sequence.MultiBattle);
                _enemyManager.PlayNpcEvent(EnemyManager.Sequence.MultiBattle);
            }
            else if (EventRunButton(4, "BossStartSeq"))
            {
                _enemyManager.BossStart();
            }
            else if (EventRunButton(5, "FirstFunnel"))
            {
                _enemyManager.FunnelExpand();
            }
            else if (EventRunButton(6, "SecondFunnel"))
            {
                _enemyManager.FunnelExpand();
            }
            else if (EventRunButton(7, "FunnelLaserSight"))
            {
                _enemyManager.FunnelLaserSight();
            }
            else if (EventRunButton(8, "MoveBossToPlayerFrontAsync"))
            {
                _enemyManager.MoveBossToPlayerFrontAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
            else if (EventRunButton(9, "BreakLeftArm"))
            {
                _enemyManager.BreakLeftArm();
            }
            else if (EventRunButton(10, "QteCombatReady"))
            {
                _enemyManager.QteCombatReady();
            }
            else if (EventRunButton(11, "FirstQteCombatAction"))
            {
                _enemyManager.FirstQteCombatAction();
            }
            else if (EventRunButton(12, "SecondQteCombatAction"))
            {
                _enemyManager.SecondQteCombatAction();
            }
            else if (EventRunButton(13, "PenetrateBoss"))
            {
                _enemyManager.PenetrateBoss();
            }
            else if (EventRunButton(14, "DefeatAllFunnel"))
            {
                FunnelController[] f = FindObjectsByType<FunnelController>(FindObjectsSortMode.None);
                foreach (FunnelController g in f) g.Damage(int.MaxValue / 2, "DebugCommand");
            }
        }

        // イベント実行のボタン
        private bool EventRunButton(int index, string label)
        {
            float x = _buttonX;
            float y = _buttonY + index * _buttonHeight;
            float w = _buttonWidth;
            float h = _buttonHeight;
            return GUI.Button(new Rect(x, y, w, h), label);
        }

        // ログの表示
        private void Log(string s)
        {
            Debug.Log($"<color=#00ff00>敵デバッグコマンド実行: {s}</color>");
        }
    }
}
