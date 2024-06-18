using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// 必要な情報を黒板に書き込む。
    /// </summary>
    public class Perception
    {
        private Transform _transform;
        private BossParams _params;
        private BlackBoard _blackBoard;
        private Transform _player;
        private Transform _pointP;
        private MeleeEquipment _meleeEquip;
        private CircleArea _area;
        private CircleArea _playerArea;

        public Perception(Transform transform, BossParams bossParams, BlackBoard blackBoard, Transform player, 
            Transform pointP, MeleeEquipment meleeEquip)
        {
            _transform = transform;
            _params = bossParams;
            _blackBoard = blackBoard;
            _player = player;
            _pointP = pointP;
            _meleeEquip = meleeEquip;
            _area = new CircleArea(transform.position, BossParams.Const.AreaRadius);
        }

        /// <summary>
        /// 初期化。
        /// エリアを作成し、他必要な値を黒板に書き込む。
        /// </summary>
        public void Init()
        {
            // プレイヤーのエリアを作成する。
            _playerArea = new CircleArea(_player.position, BossParams.Const.PlayerAreaRadius);
            
            // 黒板に書き込む。
            _blackBoard.Area = _area;
            _blackBoard.PlayerArea = _playerArea;
        }

        /// <summary>
        /// 各値を更新
        /// </summary>
        public void Update()
        {
            // 点Pの位置
            _blackBoard.PointP = _pointP.position;

            // エリアの位置をそれぞれの対象の位置に更新
            _playerArea.Point = _player.position;
            _area.Point = _transform.position;

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (_area.Collision(_playerArea)) _area.Point = _area.TouchPoint(_playerArea);

            // 自身から点Pへのベクトルを黒板に書き込む。
            _blackBoard.TransformToPointPDirection = (_blackBoard.PointP - _transform.position).normalized;
            _blackBoard.TransformToPointPDistance = (_blackBoard.PointP - _transform.position).magnitude;

            // 自身からプレイヤーへのベクトルを黒板に書き込む。
            _blackBoard.TransformToPlayerDirection = (_player.position - _transform.position).normalized;
            _blackBoard.TransformToPlayerSqrDistance = (_player.position - _transform.position).sqrMagnitude;

            // プレイヤーが近接攻撃が届く範囲にいるか。
            _blackBoard.IsWithinMeleeRange = _meleeEquip.IsWithinRange(_player.position);
            
            // ボス戦開始からの経過時間を更新。
            if (_blackBoard.IsBossStarted) _blackBoard.ElapsedTime += Time.deltaTime;
        }

        /// <summary>
        /// 後始末。
        /// このクラスが黒板に書き込んだ参照をnullにする。
        /// </summary>
        public void Dispose()
        {
            // エリアの参照をnullにする。
            _blackBoard.Area = null;
            _blackBoard.PlayerArea = null;
        }

        /// <summary>
        /// 描画。
        /// </summary>
        public void Draw()
        {
            // 自身とプレイヤーのエリア
            _area?.DrawOnGizmos();
            _playerArea?.DrawOnGizmos();
        }
    }
}
