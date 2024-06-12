using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
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
        private SlotPool _pool;

        // エリアとスロットの計算は、プレイヤーがY軸以外で回転すると破綻する可能性がある。
        private CircleArea _area;
        private CircleArea _playerArea;
        private Slot _slot;

        public Perception(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard, 
            Transform player, SlotPool pool)
        {
            _transform = transform;
            _params = enemyParams;
            _blackBoard = blackBoard;
            _player = player;
            _pool = pool;
        }

        /// <summary>
        /// 初期化。
        /// スロットを借りて、他必要な値を黒板に書き込む。
        /// </summary>
        public void Init()
        {
            // それぞれのエリアを作成
            _area = new CircleArea(_transform.position, _params.Common.Area.Radius);
            _playerArea = new CircleArea(_player.position, _params.Common.Area.PlayerRadius);
            // スロット確保
            _pool.TryRent(_transform.position, out _slot);

            // エリアとスロットを黒板に書き込む。
            _blackBoard.Area = _area;
            _blackBoard.PlayerArea = _playerArea;
            _blackBoard.Slot = _slot;

            // 生存時間の初期値を黒板に書き込む。
            _blackBoard.LifeTime = _params.Battle.LifeTime;
        }

        /// <summary>
        /// 各値を更新
        /// </summary>
        public void Update()
        {
            // エリアの位置をそれぞれの対象の位置に更新。
            _playerArea.Point = _player.position;
            _area.Point = _transform.position;

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (_area.Collision(_playerArea)) _area.Point = _area.TouchPoint(_playerArea);

            // プレイヤーの位置を黒板に書き込む。
            _blackBoard.PlayerPosition = _player.position;

            // 自身からプレイヤーへのベクトルを黒板に書き込む。
            _blackBoard.TransformToPlayerDirection = (_player.position - _transform.position).normalized;
            _blackBoard.TransformToPlayerDistance = (_player.position - _transform.position).magnitude;

            // 自身とプレイヤーの距離が検知可能範囲内の場合は、プレイヤーを検知したフラグを立てる。
            if (_blackBoard.TransformToPlayerDistance < _params.Advance.Distance)
            {
                _blackBoard.IsPlayerDetected = true;
            }

            // プレイヤーを検知した状態ならば生存時間を減らす。
            if (_blackBoard.IsPlayerDetected) _blackBoard.LifeTime -= _blackBoard.PausableDeltaTime;

            // 自身のエリアからスロットへのベクトルを黒板に書き込む。
            _blackBoard.AreaToSlotDirection = (_slot.Point - _area.Point).normalized;
            _blackBoard.AreaToSlotSqrDistance = (_slot.Point - _area.Point).sqrMagnitude;

            // スロットに到着した場合は、接近完了フラグを立てる。
            if (_blackBoard.AreaToSlotSqrDistance < EnemyParams.Const.ApproachCompleteThreshold)
            {
                _blackBoard.IsApproachCompleted = true;
            }
        }

        /// <summary>
        /// 後始末。
        /// スロットを返却し、このクラスが黒板に書き込んだ参照をnullにする。
        /// </summary>
        public void Dispose()
        {
            if (_pool != null) _pool.Return(_slot);

            _blackBoard.Area = null;
            _blackBoard.PlayerArea = null;
            _blackBoard.Slot = null;
        }

        /// <summary>
        /// 描画。
        /// </summary>
        public void Draw()
        {
            // 自身とプレイヤーのエリア
            _playerArea?.DrawOnGizmos();
            _area?.DrawOnGizmos();

            // エリアからスロットに向けた矢印
            if (_slot != null)
            {
                GizmosUtils.Line(_area.Point, _slot.Point, ColorExtensions.ThinWhite);
            }

            // プレイヤーを検知する範囲
            GizmosUtils.WireCircle(_transform.position, _params.Advance.Distance, ColorExtensions.ThinGreen);
        }
    }
}
