using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 位置関係を調べる。
    /// プレイヤーがY軸以外で回転すると破綻する可能性がある。
    /// </summary>
    public class PositionRelationship
    {
        private Transform _transform;
        private Transform _rotate;
        private Transform _player;
        private SlotPool _pool;
        private EnemyParams _params;

        private CircleArea _area;
        private CircleArea _playerArea;
        // スロットの位置の変更はこのクラスでは行わず、貸し出す側が行う。
        private Slot _slot;

        public PositionRelationship(Transform transform, Transform rotate, Transform player, SlotPool pool, 
            EnemyParams enemyParams)
        {
            _transform = transform;
            _rotate = rotate;
            _player = player;
            _pool = pool;
            _params = enemyParams;           
        }

        /// <summary>
        /// 各インスタンスの確保を行う。
        /// AreaFixメソッドで更新する前に一度必ず呼ぶ。
        /// </summary>
        public void Setup(BlackBoard blackBoard)
        {
            // このキャラクターのエリアを作成
            _area = new CircleArea(_transform.position, _params.Common.Area.Radius);

            // プレイヤーのエリアを作成
            if (_player != null)
            {
                _playerArea = new CircleArea(_player.position, _params.Common.Area.PlayerRadius);
            }

            // スロット確保
            if (_pool == null)
            {
                Debug.LogError($"敵を配置するスロットの確保に失敗: {_transform.name}");
            }
            else
            {
                _slot = _pool.Rent(_params.Advance.Slot);
            }

            // 参照させ、AreaFixメソッドで位置を書き換えていく。
            blackBoard.Area = _area;
            blackBoard.PlayerArea = _playerArea;
            blackBoard.Slot = _slot;

            PlayerWith(blackBoard);
        }

        /// <summary>
        /// 各値を更新
        /// </summary>
        public void Update(BlackBoard blackBoard)
        {
            AreaFix();
            PlayerWith(blackBoard);
            SlotWith(blackBoard);
        }

        // 自身とプレイヤーのエリアが被らないように位置を修正する。
        void AreaFix()
        {
            if (_player == null) return;

            // エリアの位置をそれぞれの対象の位置に更新
            _playerArea.Point = _player.position;
            _area.Point = _transform.position;

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (_area.Collision(_playerArea)) _area.Point = _area.TouchPoint(_playerArea);
        }

        // プレイヤーとの位置関係を更新する。
        void PlayerWith(BlackBoard blackBoard)
        {
            if (_player == null) return;

            // プレイヤーの位置
            blackBoard.PlayerPosition = _player.position;

            // 自身からプレイヤーへ
            Vector3 toPlayer = _player.position - _transform.position;
            blackBoard.TransformToPlayerDirection = toPlayer.normalized;
            blackBoard.TransformToPlayerDistance = toPlayer.magnitude;
        }

        // スロットとの位置関係を更新する。
        void SlotWith(BlackBoard blackBoard)
        {
            if (_slot == null) return;

            // エリアとスロット
            blackBoard.AreaToSlotDirection = (_slot.Point - _area.Point).normalized;
            blackBoard.AreaToSlotSqrDistance = (_slot.Point - _area.Point).sqrMagnitude;

            // スロットに到着した際に接近完了フラグを立てる。
            if (blackBoard.AreaToSlotSqrDistance < EnemyParams.Debug.ApproachCompleteThreshold)
            {
                blackBoard.IsApproachCompleted = true;
            }
        }

        /// <summary>
        /// 後始末。
        /// スロットを返却し、このクラスが代入した参照をnullにする。
        /// </summary>
        public void Erase(BlackBoard blackBoard)
        {
            if (_pool != null) _pool.Return(_slot);

            blackBoard.Area = null;
            blackBoard.PlayerArea = null;
            blackBoard.Slot = null;
        }

        /// <summary>
        /// 各エリアの描画
        /// </summary>
        public void DrawArea()
        {
            _playerArea?.DrawOnGizmos();
            _area?.DrawOnGizmos();

            // エリアからスロットに向けた矢印
            if (_slot != null)
            {
                GizmosUtils.Line(_area.Point, _slot.Point, ColorExtensions.ThinWhite);
            }

            // 前方向
            GizmosUtils.Line(_transform.position, _transform.position + _rotate.forward, Color.blue);
        }
    }
}
