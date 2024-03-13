using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 自身の状態や周囲を認識して黒板に書き込む。
    /// </summary>
    public class Perception : ILifeCycleHandler
    {
        private PositionRelationship _position;
        private FovSensor _fovSensor;
        private BlackBoard _blackBoard;

        public Perception(Transform transform, Transform rotate, Transform player, EnemyParams enemyParams, 
            BlackBoard blackBoard, SurroundingPool pool)
        {
            _position = new PositionRelationship(transform, player, pool, enemyParams);
            _fovSensor = new FovSensor(transform, rotate, enemyParams);
            _blackBoard = blackBoard;
        }

        public void OnStartEvent()
        {
            _position.Setup(_blackBoard);
        }

        public void OnEnableEvent()
        {
            _fovSensor.OnCaptureEnter += Enter;
            _fovSensor.OnCaptureStay += Stay;
            _fovSensor.OnCaptureExit += Exit;
        }

        public void OnDisableEvent()
        {
            _fovSensor.OnCaptureEnter -= Enter;
            _fovSensor.OnCaptureStay -= Stay;
            _fovSensor.OnCaptureExit -= Exit;

            // 撃破演出後に無効化して画面から消す想定。
            _position.Erase(_blackBoard);
        }

        public void UpdateEvent()
        {
            _fovSensor.CheckFOV();
            _position.AreaFix();
        }

        public void LateUpdateEvent()
        {
            // Updateで登録したコールバックが呼ばれる事が前提条件。
            // 黒板に書き込んだ内容をフレームを跨ぐ前に全て消す。
            _blackBoard.FovEnter.Clear();
            _blackBoard.FovStay.Clear();
            _blackBoard.FovExit.Clear();
        }

        public void OnDrawGizmosEvent()
        {
            _fovSensor.DrawViewRange();
            _position.DrawArea();

            // LateUpdateで書き込んだ内容を消しているので、黒板の情報は正常に描画されない。
        }

        private void Enter(Collider collider)
        {
            _blackBoard.FovEnter.Add(collider);
        }

        private void Stay(Collider collider)
        {
            _blackBoard.FovStay.Add(collider);
        }

        private void Exit(Collider collider)
        {
            _blackBoard.FovExit.Add(collider);
        }
    }
}