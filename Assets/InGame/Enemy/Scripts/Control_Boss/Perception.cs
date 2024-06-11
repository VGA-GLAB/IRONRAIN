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
        private BlackBoard _blackBoard;
        private Transform _player;
        private Transform _pointP;
        private CircleArea _area;
        private CircleArea _playerArea;

        public Perception(Transform transform, BlackBoard blackBoard, Transform player, Transform pointP)
        {
            _transform = transform;
            _blackBoard = blackBoard;
            _player = player;
            _pointP = pointP;
            _area = new CircleArea(transform.position, BossParams.Debug.AreaRadius);
        }

        /// <summary>
        /// 初期化。
        /// エリアを作成し、他必要な値を黒板に書き込む。
        /// </summary>
        public void Init()
        {
            // プレイヤーのエリアを作成する。
            _playerArea = new CircleArea(_player.position, BossParams.Debug.PlayerAreaRadius);
            
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

            // ボス戦開始からの経過時間を更新
            _blackBoard.ElapsedTime += Time.deltaTime;
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
