using System;
using UniRx;
using UnityEngine;
using Enemy.Control;

namespace Enemy.Unused
{
    // 現在未使用
    /// <summary>
    /// レベルの調整のメッセージを受信する。
    /// </summary>
    public class LevelAdjust
    {
        private BlackBoard _blackBoard;

        // 受信したタイミングで黒板に書き込むのではなく、一旦変数に保持して任意のタイミングで書き込みを行う。
        private LevelAdjustMessage _temp;
        // GameObjectに紐づけなくても任意のタイミングで開放できる。
        private IDisposable _disposable;

        public LevelAdjust(Transform transform, BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            
            Receive(transform);
        }

        // メッセージを受信させる。
        private void Receive(Transform transform)
        {
            // デフォルト値を一旦保持し、最初に受信するまではこの値が書き込まれる。
            _temp = new LevelAdjustMessage { MoveSpeed = 0, FireRate = 0 };
            
            _disposable = MessageBroker.Default.Receive<LevelAdjustMessage>()
                .Subscribe(msg => _temp = msg).AddTo(transform);
        }

        /// <summary>
        /// 一時保存キューに保持した内容を黒板に書き込む。
        /// </summary>
        public void Write()
        {
            // 最後に受信した値を一旦保持しているので、次の受信のタイミングまで値が更新されない。
            //_blackBoard.LevelAdjust = _temp; <- ないなった
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
