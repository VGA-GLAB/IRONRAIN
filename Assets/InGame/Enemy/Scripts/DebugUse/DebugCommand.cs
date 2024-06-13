using Enemy.Control;
using System;
using System.Collections;
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
        // コマンド実行時の演出を表示
        private bool _isEnterEffect;

        private void Awake()
        {
            _style.fontSize = 30;
            _state.textColor = Color.green;
            _style.normal = _state;
        }

        private void Update()
        {
            // 有効/無効を切り替え
            if (Input.GetKeyDown(KeyCode.F1))
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

            // コマンドを実行
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // コマンドを実行して成功した場合は演出
                if (RunCommand()) StartCoroutine(EnterEffectAsync());

                _command.Clear();

            }
        }

        // コマンド実行の演出。
        // フラグをオンオフ繰り返して演出する。
        private IEnumerator EnterEffectAsync()
        {
            for (int i = 0; i < 6; i++)
            {
                _isEnterEffect = !_isEnterEffect;
                yield return new WaitForSeconds(0.15f);
            }
        }

        // コマンドを実行し、成功/失敗を返す。
        bool RunCommand()
        {
            string[] cmd = _command.ToString().Split();

            // 生存中の敵の数を数える。
            if (cmd[0] == "ENEMYCOUNT")
            {
                Debug.Log($"生存中の敵の数{_enemyManager.EnemyCount()}");
                return true;
            }

            // シーケンスの敵が全滅しているかチェック
            if (cmd[0] == "ISALLDEFEATED" && cmd.Length == 2)
            {
                EnemyManager.Sequence seq;
                if (cmd[1] == "NONE") seq = EnemyManager.Sequence.None;
                else if (cmd[1] == "FIRSTANAUNNCE") seq = EnemyManager.Sequence.FirstAnaunnce;
                else if (cmd[1] == "MULTIBATTLE") seq = EnemyManager.Sequence.MultiBattle;
                else return false;

                Debug.Log($"{seq}シーケンスの敵が全滅している: {_enemyManager.IsAllDefeated(seq)}");
                return true;
            }

            // プレイヤーを検出
            if (cmd[0] == "DETECTPLAYER" && cmd.Length == 2)
            {
                EnemyManager.Sequence seq;
                if (cmd[1] == "NONE") seq = EnemyManager.Sequence.None;
                else if (cmd[1] == "FIRSTANAUNNCE") seq = EnemyManager.Sequence.FirstAnaunnce;
                else if (cmd[1] == "MULTIBATTLE") seq = EnemyManager.Sequence.MultiBattle;
                else return false;

                _enemyManager.DetectPlayer(seq);
                Debug.Log($"{seq}シーケンスの敵がプレイヤーを検出");
                return true;
            }

            // シーケンスの敵を全滅させる
            if (cmd[0] == "DEFEATTHEMALL" && cmd.Length == 2)
            {
                EnemyManager.Sequence seq;
                if (cmd[1] == "NONE") seq = EnemyManager.Sequence.None;
                else if (cmd[1] == "FIRSTANAUNNCE") seq = EnemyManager.Sequence.FirstAnaunnce;
                else if (cmd[1] == "MULTIBATTLE") seq = EnemyManager.Sequence.MultiBattle;
                else return false;

                _enemyManager.DefeatThemAll(seq);
                Debug.Log($"{seq}シーケンスの敵を全滅");
                return true;
            }

            // ボス戦開始
            if (cmd[0] == "BOSSSTART" && cmd.Length == 1)
            {
                _enemyManager.BossStart();
                Debug.Log($"ボス戦開始");
                return true;
            }

            // NPCのシーケンスイベント
            if (cmd[0] == "PLAYNPCEVENT" && cmd.Length == 2)
            {
                EnemyManager.Sequence seq;
                if (cmd[2] == "NONE") seq = EnemyManager.Sequence.None;
                else if (cmd[2] == "MULTIBATTLE") seq = EnemyManager.Sequence.MultiBattle;
                else return false;

                _enemyManager.PlayNpcEvent(seq);
                Debug.Log($"{seq}シーケンスのNPCイベントを実行");
                return true;
            }

            return false;
        }

        private void OnGUI()
        {
            if (_isEnable)
            {
                GUILayout.Label($"ｺﾏﾝﾄﾞ: {_command}<", _style);
            }

            if (_isEnterEffect)
            {
                GUILayout.Label($"ｺﾏﾝﾄﾞｼﾞｯｺｳ", _style);
            }
        }
    }
}
