using Enemy.Control;
using UniRx;
using UnityEngine;

namespace Enemy.Unused
{
    /// <summary>
    /// ゲームのレベルを調整するAIとのやりとりを描画する。
    /// </summary>
    public class CombatDesignAiDebugger : MonoBehaviour
    {
        private GUIStyle _style = new GUIStyle();
        private GUIStyleState _state = new GUIStyleState();

        // セットした値を表示するだけ
        public bool UseAI { get; set; }
        public string Response { get; set; }

        private void Awake()
        {
            _style.fontSize = 30;
            _state.textColor = Color.white;
            _style.normal = _state;
        }

        private void Update()
        {
            // AIを使用しなくても動くよう、キー入力でメッセージングを行う。
            if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
            {
                MessageBroker.Default.Publish(new LevelAdjustMessage
                {
                    FireRate = 1.0f,
                    MoveSpeed = 1.0f,
                });
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                MessageBroker.Default.Publish(new LevelAdjustMessage
                {
                    FireRate = -0.5f,
                    MoveSpeed = -0.5f,
                });
            }
        }

        private void OnGUI()
        {
            GUILayout.Label($"AI使用: {UseAI}", _style);
            GUILayout.Label($"レスポンス: {Response}", _style);
        }
    }
}