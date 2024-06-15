using Enemy.Control;
using Enemy.Extensions;
using System;
using System.Text;
using UnityEngine;

namespace Enemy.DebugUse
{
    public class DebugCommand : MonoBehaviour
    {
        [Header("コマンドで敵やNPCに命令する")]
        [SerializeField] private EnemyManager _enemyManager;

        private GUIStyle _style = new GUIStyle();
        private GUIStyleState _state = new GUIStyleState();
        private StringBuilder _command = new StringBuilder();
        
        // コマンド入力可能状態
        private bool _isEnable;

        private void Awake()
        {
            _style.fontSize = 30;
            _state.textColor = Color.green;
            _style.normal = _state;
        }

        private void Update()
        {
            // 有効/無効を切り替え
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                _command.Clear();
                _isEnable = !_isEnable;
            }

            if (!_isEnable) return;

            // 何らかのキーが入力されている場合はコマンドに追加
            if (Input.anyKeyDown)
            {
                foreach (KeyCode c in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(c))
                    {
                        // アルファベット。
                        if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c.ToString()))
                        {
                            _command.Append(c);
                        }
                        // Alpha0~9の最後の1文字を数字として扱う。
                        else if (c.ToString().Length == 6 && int.TryParse(c.ToString()[^1].ToString(), out int i))
                        {
                            _command.Append(i);
                        }
                        // スペース。
                        else if (c == KeyCode.Space)
                        {
                            _command.Append(" ");
                        }
                    }
                }
            }

            // 1文字消す。
            if (Input.GetKeyDown(KeyCode.Backspace) && _command.Length > 0)
            {
                _command.Remove(_command.Length - 1, 1);
            }

            // コマンドを実行
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RunCommand();
                _command.Clear();

            }
        }

        // コマンドを実行し、成功/失敗を返す。
        bool RunCommand()
        {
            string[] cmd = _command.ToString().Split();

            // 生存中の敵の数を数える。
            if (cmd[0] == "ENEMYCOUNT" && cmd.Length == 1)
            {
                Log($"生存中の敵の数 {_enemyManager.EnemyCount()}");
                return true;
            }

            // シーケンスの敵が全滅しているかチェック
            if (cmd[0] == "ISALLDEFEATED" && cmd.Length == 2)
            {
                if (TryToSequence(cmd[1], out EnemyManager.Sequence seq))
                {
                    Log($"{seq}シーケンスの敵が全滅 {_enemyManager.IsAllDefeated(seq)}");
                    return true;
                }

                return false;
            }

            // プレイヤーを検出
            if (cmd[0] == "DETECTPLAYER" && cmd.Length == 2)
            {
                if (TryToSequence(cmd[1], out EnemyManager.Sequence seq))
                {
                    _enemyManager.DetectPlayer(seq);
                    Log($"{seq}シーケンスの敵がプレイヤーを検出");
                    return true;
                }

                return false;
            }

            // シーケンスの敵を全滅させる
            if (cmd[0] == "DEFEATTHEMALL" && cmd.Length == 2)
            {
                if (TryToSequence(cmd[1], out EnemyManager.Sequence seq))
                {
                    _enemyManager.DefeatThemAll(seq);
                    Log($"{seq}シーケンスの敵を全滅");
                    return true;
                }

                return false;
            }

            // ボス戦開始
            if (cmd[0] == "BOSSSTART" && cmd.Length == 1)
            {
                _enemyManager.BossStart();
                Log($"ボス戦開始");
                return true;
            }

            // プレイヤーの左腕破壊
            if (cmd[0] == "BREAKLEFTARM" && cmd.Length == 1)
            {
                _enemyManager.BreakLeftArm();
                Log($"プレイヤーの左腕破壊イベント開始");
                return true;
            }

            // ボス戦1回目のQTE
            if (cmd[0] == "BOSSFIRSTQTE" && cmd.Length == 1)
            {
                _enemyManager.BossFirstQte();
                Log($"ボス戦1回目のQTE開始");
                return true;
            }

            // ボス戦2回目のQTE
            if (cmd[0] == "BOSSSECONDQTE" && cmd.Length == 1)
            {
                _enemyManager.BossSecondQte();
                Log($"ボス戦2回目のQTE開始");
                return true;
            }

            // NPCのシーケンスイベント
            if (cmd[0] == "PLAYNPCEVENT" && cmd.Length == 2)
            {
                if (TryToSequence(cmd[1], out EnemyManager.Sequence seq))
                {
                    _enemyManager.PlayNpcEvent(seq);
                    Log($"{seq}シーケンスのNPCイベントを実行");
                    return true;
                }

                return false;
            }

            return false;
        }

        // 列挙型に変換
        private bool TryToSequence(string s, out EnemyManager.Sequence value)
        {
            foreach (EnemyManager.Sequence seq in EnumExtensions.GetAll<EnemyManager.Sequence>())
            {
                if (seq.ToString().ToUpper() == s)
                {
                    value = seq;
                    return true;
                }
            }

            value = EnemyManager.Sequence.None;
            return false;
        }

        // ログの表示
        private void Log(string s)
        {
            Debug.Log($"<color=#00ff00>敵デバッグコマンド実行: {s}</color>");
        }

        private void OnGUI()
        {
            if (_isEnable)
            {
                GUILayout.Label($"ｺﾏﾝﾄﾞ: {_command}<", _style);
            }
        }
    }
}
