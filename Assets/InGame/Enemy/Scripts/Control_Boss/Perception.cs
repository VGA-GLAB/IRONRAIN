using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class Perception : MonoBehaviour
    {
        private Transform _transform;
        private Transform _player;
        private BlackBoard _blackBoard;
        private BossStage _stage;
        private CircleArea _area;
        private CircleArea _playerArea;

        public Perception(Transform transform, Transform player, BlackBoard blackBoard, BossStage stage)
        {
            _transform = transform;
            _player = player;
            _blackBoard = blackBoard;
            _stage = stage;
            _area = new CircleArea(transform.position, BossParams.Debug.AreaRadius);
        }

        public void OnStartEvent()
        {
            _playerArea = new CircleArea(_player.position, BossParams.Debug.PlayerAreaRadius);
            _blackBoard.Area = _area;
            _blackBoard.PlayerArea = _playerArea;
        }

        public void UpdateEvent()
        {
            // 点Pの位置
            _blackBoard.PointP = _stage.PointP.position;

            // エリアの位置をそれぞれの対象の位置に更新
            _playerArea.Point = _player.position;
            _area.Point = _transform.position;

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (_area.Collision(_playerArea)) _area.Point = _area.TouchPoint(_playerArea);
        }

        public void OnDisableEvent()
        {
            // エリアの参照をnullにする。
            _blackBoard.Area = null;
            _blackBoard.PlayerArea = null;
        }

        public void OnDrawGizmosEvent()
        {
            _area?.DrawOnGizmos();
            _playerArea?.DrawOnGizmos();
        }
    }
}
