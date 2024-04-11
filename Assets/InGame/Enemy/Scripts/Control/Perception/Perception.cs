using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 自身の状態や周囲を認識して黒板に書き込む。
    /// </summary>
    public class Perception : LifeCycle
    {
        private LevelAdjust _levelAdjust;
        private PlayerInput _playerInput;
        private FireRate _fireRate;
        private PositionRelationship _position;
        private FovSensor _fovSensor;
        private ConditionCheck _conditionCheck;
        private BlackBoard _blackBoard;

        public Perception(Transform transform, Transform rotate, Transform player, EnemyParams enemyParams, 
            BlackBoard blackBoard, SurroundingPool pool)
        {
            _levelAdjust = new LevelAdjust(transform, blackBoard);
            _playerInput = new PlayerInput(transform, blackBoard);
            _fireRate = new FireRate(enemyParams, blackBoard);
            _position = new PositionRelationship(transform, rotate, player, pool, enemyParams);
            _fovSensor = new FovSensor(transform, rotate, enemyParams);
            _conditionCheck = new ConditionCheck(transform, enemyParams, blackBoard);
            _blackBoard = blackBoard;
        }

        public override void OnStartEvent()
        {
            _position.Setup(_blackBoard);
            _conditionCheck.Setup();
        }

        public override void OnEnableEvent()
        {
            _fovSensor.OnCaptureEnter += Enter;
            _fovSensor.OnCaptureStay += Stay;
            _fovSensor.OnCaptureExit += Exit;
        }

        public override void OnDisableEvent()
        {
            _fovSensor.OnCaptureEnter -= Enter;
            _fovSensor.OnCaptureStay -= Stay;
            _fovSensor.OnCaptureExit -= Exit;

            // 撃破演出後に無効化して画面から消す想定。
            _position.Erase(_blackBoard);
        }

        public override Result UpdateEvent()
        {
            // 黒板への読み書きに使う様々なパラメータを増減させる可能性があるので
            // メッセージの受信は一番最初に実行しておく。
            _levelAdjust.Write();
            _playerInput.Write();

            _fovSensor.CheckFOV();
            _position.AreaFix();
            _position.PlayerWith(_blackBoard);
            _position.SlotWith(_blackBoard);
            _fireRate.NextTiming();
            _conditionCheck.Check();

            return Result.Running;
        }

        public override Result LateUpdateEvent()
        {
            _playerInput.Clear();

            // Updateで登録したコールバックが呼ばれる事が前提条件。
            // 黒板に書き込んだ内容をフレームを跨ぐ前に全て消す。
            _blackBoard.FovEnter.Clear();
            _blackBoard.FovStay.Clear();
            _blackBoard.FovExit.Clear();

            return Result.Running;
        }

        public override void OnDrawGizmosEvent()
        {
            _fovSensor.DrawViewRange();
            _position.DrawArea();

            // LateUpdateで書き込んだ内容を消しているので、黒板の情報は正常に描画されない。
        }

        public override void OnDamaged(int value, string weapon)
        {
            _conditionCheck.Damage(value, weapon);
        }

        public override void OnPreCleanup()
        {
            _levelAdjust.Dispose();
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
