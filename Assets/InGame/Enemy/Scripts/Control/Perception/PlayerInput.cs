using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// プレイヤーの入力のメッセージを受信する。
    /// </summary>
    public class PlayerInput
    {
        private BlackBoard _blackBoard;

        // 受信したタイミングで黒板に書き込むのではなく、一旦キューイングして任意のタイミングで書き込みを行う。
        private Queue<PlayerInputMessage> _temp;
        // GameObjectに紐づけなくても任意のタイミングで開放できる。
        private IDisposable _disposable;

        public PlayerInput(Transform transform, BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _temp = new Queue<PlayerInputMessage>();

            Receive(transform);
        }

        // メッセージを受信させる。
        private void Receive(Transform transform)
        {
            _disposable = MessageBroker.Default.Receive<PlayerInputMessage>()
                .Subscribe(msg => _temp.Enqueue(msg)).AddTo(transform);
        }

        /// <summary>
        /// 一時保存キューに保持した内容を黒板に書き込む。
        /// </summary>
        public void Write()
        {
            // 最後に受信した値を一旦保持しているので、次の受信のタイミングまで値が更新されない。
            while(_temp.TryDequeue(out PlayerInputMessage msg))
            {
                _blackBoard.PlayerInput.Enqueue(msg);
            }
        }

        /// <summary>
        /// 黒板に書き込んだ内容を消す。
        /// </summary>
        public void Clear()
        {
            _blackBoard.PlayerInput.Clear();
        }

        /// <summary>
        /// メッセージ受信登録を破棄することでこれ以上受信させない。
        /// </summary>
        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
