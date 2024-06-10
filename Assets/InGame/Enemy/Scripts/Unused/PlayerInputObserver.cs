using UnityEngine;
using UniRx;

namespace Enemy.Unused
{
    /// <summary>
    /// 入力機器の変化に対応させるため、プレイヤーの入力をこの列挙型に紐づけ、送信する。
    /// </summary>
    public enum PlayerActionMap
    {
        None,
        Attack,
    }

    /// <summary>
    /// プレイヤーの入力を敵キャラクターに共有するメッセージの構造体
    /// </summary>
    public struct PlayerInputMessage
    {
        public PlayerActionMap Map;
    }

    /// <summary>
    /// プレイヤーの入力を監視し、敵キャラクターに共有する。
    /// </summary>
    public class PlayerInputObserver : MonoBehaviour
    {
        private void Update()
        {
            // 任意のタイミングでPublishメソッドを呼ぶ。
        }

        // メッセージング
        private void Publish(PlayerActionMap map)
        {
            MessageBroker.Default.Publish(new PlayerInputMessage { Map = map });
        }
    }
}
