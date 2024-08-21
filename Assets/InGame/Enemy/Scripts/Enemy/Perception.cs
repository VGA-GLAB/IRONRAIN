using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 必要な情報を黒板に書き込む。
    /// </summary>
    public class Perception
    {
        private Transform _transform;
        private EnemyParams _params;
        private BlackBoard _blackBoard;
        private Transform _player;

        // 破棄した後に更新処理がされないようのフラグ。
        private bool _isDisposed;

        public Perception(RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;
            _player = requiredRef.Player;
        }

        /// <summary>
        /// 初期化。
        /// スロットを借りて、他必要な値を黒板に書き込む。
        /// </summary>
        public void Init()
        {
            // エリアとスロットを作成、黒板に書き込む。
            _blackBoard.Area = AreaCalculator.CreateArea(_transform.position);
            _blackBoard.PlayerArea = AreaCalculator.CreatePlayerArea(_player);
            _blackBoard.Slot = AreaCalculator.CreateSlot(_player, _params.Slot);

            // 生存時間の初期値を黒板に書き込む。
            _blackBoard.LifeTime = _params.LifeTime;
        }

        /// <summary>
        /// 各値を更新。
        /// </summary>
        public void Update()
        {
            if (_isDisposed || _blackBoard.CurrentState == FSM.StateKey.Hide) return;

            // エリアとスロットの位置を更新。
            _blackBoard.Area.Point = AreaCalculator.AreaPoint(_transform);
            _blackBoard.PlayerArea.Point = AreaCalculator.AreaPoint(_player);
            _blackBoard.Slot.Point = AreaCalculator.SlotPoint(_player, _params.Slot);

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (_blackBoard.Area.Collision(_blackBoard.PlayerArea))
            {
                _blackBoard.Area.Point = _blackBoard.Area.TouchPoint(_blackBoard.PlayerArea);
            }

            // 自身からプレイヤーへのベクトルを黒板に書き込む。
            Vector3 pv = _player.position - _transform.position;
            float pMag = pv.magnitude;
            _blackBoard.PlayerDistance = pMag;
            _blackBoard.PlayerDirection = pMag > 1E-05F ? pv / pMag : Vector3.zero;

                // プレイヤーを検知した状態ならば生存時間を減らす。
            if (_blackBoard.IsPlayerDetect) _blackBoard.LifeTime -= _blackBoard.PausableDeltaTime;

            // 自身のエリアからスロットへのベクトルを黒板に書き込む。
            Vector3 sv = _blackBoard.Slot.Point - _blackBoard.Area.Point;
            _blackBoard.SlotDirection = sv.normalized;
            _blackBoard.SlotSqrDistance = sv.sqrMagnitude;

            // スロットに到着した場合は、接近完了フラグを立てる。
            if (_blackBoard.SlotSqrDistance < _params.Other.ApproachCompleteThreshold)
            {
                _blackBoard.IsApproachCompleted = true;
            }
        }

        /// <summary>
        /// 後始末。
        /// このクラスが黒板に書き込んだ参照をnullにする。
        /// </summary>
        public void Dispose()
        {
            _blackBoard.Area = null;
            _blackBoard.PlayerArea = null;
            _blackBoard.Slot = null;

            _isDisposed = true;
        }

        /// <summary>
        /// 描画。
        /// </summary>
        public void Draw()
        {
            _blackBoard.PlayerArea?.Draw();
            _blackBoard.Area?.Draw();
            _blackBoard.Slot?.Draw();
        }
    }
}
