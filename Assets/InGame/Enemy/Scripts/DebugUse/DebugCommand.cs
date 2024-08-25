using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;
using Cysharp.Threading.Tasks;

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
        // 配置されている敵のシーケンスID一覧。
        private List<int> _sequenceID;

        private void Awake()
        {
            _enemyManager = GetComponent<EnemyManager>();
            _sequenceID = new List<int>();
        }

        private void Update()
        {
            // 有効/無効を切り替え
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                _isEnable = !_isEnable;
                CheckSequenceID();
            }
        }

        private void OnGUI()
        {
            if (_isEnable) EventRunButton();
        }

        private void CheckSequenceID()
        {
            _sequenceID.Clear();

            // プランナーが0から連番でシーケンスIDを割り当ててくれる想定。
            for (int i = 0; i < 100; i++) 
            {
                if (_enemyManager.TryGetEnemies(i, new List<EnemyController>()))
                {
                    _sequenceID.Add(i);
                }
            }
        }

        // イベント実行のボタン
        private void EventRunButton()
        {
            // シーン上の敵を呼び出し。
            for (int i = 0; i < _sequenceID.Count; i++)
            {
                if (EventRunButton(i, $"シーケンス {_sequenceID[i]} の敵を生成"))
                {
                    _enemyManager.Spawn(_sequenceID[i]);
                }
            }

            // 道中のイベント
            float offset = _buttonWidth;
            if (EventRunButton(0, "味方機体のイベント再生", offset))
            {
                _enemyManager.Spawn(3);
                _enemyManager.PlayNpcEvent(3);
            }

            // ボス戦関係のコマンド
            offset = _buttonWidth * 2;
            if (EventRunButton(0, "ボス戦開始", offset))
            {
                _enemyManager.BossStart();
            }
            else if (EventRunButton(1, "1回目のファンネル展開", offset))
            {
                _enemyManager.FunnelExpand();
            }
            else if (EventRunButton(2, "2回目のファンネル展開", offset))
            {
                _enemyManager.FunnelExpand();
            }
            else if (EventRunButton(3, "ファンネルのレーザーサイト表示", offset))
            {
                _enemyManager.FunnelLaserSight();
            }
            else if (EventRunButton(4, "ファンネルを全て破壊", offset))
            {
                FunnelController[] f = FindObjectsByType<FunnelController>(FindObjectsSortMode.None);
                foreach (FunnelController g in f) g.Damage(int.MaxValue / 2, "DebugCommand");
            }

            // ボス戦QTEのコマンド
            offset = _buttonWidth * 3;
            if (EventRunButton(0, "プレイヤーの正面まで移動", offset))
            {
                _enemyManager.MoveBossToPlayerFrontAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
            else if (EventRunButton(1, "左腕破壊", offset))
            {
                _enemyManager.BreakLeftArm();
            }
            else if (EventRunButton(2, "構え直し", offset))
            {
                _enemyManager.QteCombatReady();
            }
            else if (EventRunButton(3, "1回目の鍔迫り合い", offset))
            {
                _enemyManager.FirstQteCombatAction();
            }
            else if (EventRunButton(4, "2回目の鍔迫り合い", offset))
            {
                _enemyManager.SecondQteCombatAction();
            }
            else if (EventRunButton(5, "貫かれて死亡", offset))
            {
                _enemyManager.PenetrateBoss();
            }
        }

        // イベント実行のボタン
        private bool EventRunButton(int index, string label, float xOffset = 0)
        {
            float x = _buttonX + xOffset;
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
