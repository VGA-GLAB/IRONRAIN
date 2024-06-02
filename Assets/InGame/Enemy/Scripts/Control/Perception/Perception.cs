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
        private ApproachSensor _approachSensor;
        private ConditionCheck _conditionCheck;
        private ExternalTiming _externalTiming;
        private BlackBoard _blackBoard;

        public Perception(Transform transform, Transform rotate, Transform player, EnemyParams enemyParams, 
            BlackBoard blackBoard, SlotPool pool)
        {
            _levelAdjust = new LevelAdjust(transform, blackBoard);
            _playerInput = new PlayerInput(transform, blackBoard);
            _fireRate = new FireRate(enemyParams, blackBoard);
            _position = new PositionRelationship(transform, rotate, player, pool, enemyParams);
            _fovSensor = new FovSensor(transform, rotate, enemyParams);
            _approachSensor = new ApproachSensor(transform, enemyParams);
            _conditionCheck = new ConditionCheck(transform, enemyParams, blackBoard);
            _externalTiming = new ExternalTiming(blackBoard);
            _blackBoard = blackBoard;
        }

        public override void OnStartEvent()
        {
            _position.Setup(_blackBoard);
            _conditionCheck.Setup();
        }

        public override void OnEnableEvent()
        {
            _fovSensor.OnCaptureEnter += FovCaptureEnter;
            _fovSensor.OnCaptureStay += FovCaptureStay;
            _fovSensor.OnCaptureExit += FovCaptureExit;
        }

        public override void OnDisableEvent()
        {
            _fovSensor.OnCaptureEnter -= FovCaptureEnter;
            _fovSensor.OnCaptureStay -= FovCaptureStay;
            _fovSensor.OnCaptureExit -= FovCaptureExit;

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
            _approachSensor.Update(_blackBoard);
            _position.Update(_blackBoard);
            _fireRate.NextTiming();
            _conditionCheck.Check();

            // 外部からの操作によってポーズするフラグも黒板に反映している。
            _externalTiming.Update();

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
            _approachSensor.DrawRange();
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

        public override void OnAttackEvent()
        {
            _externalTiming.AttackTrigger();
        }

        public override void OnPauseEvent()
        {
            _externalTiming.Pause();
        }

        public override void OnResumeEvent()
        {
            _externalTiming.Resume();
        }

        private void FovCaptureEnter(Collider collider)
        {
            _blackBoard.FovEnter.Add(collider);
        }

        private void FovCaptureStay(Collider collider)
        {
            _blackBoard.FovStay.Add(collider);
        }

        private void FovCaptureExit(Collider collider)
        {
            _blackBoard.FovExit.Add(collider);
        }
    }
}
