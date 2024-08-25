using UnityEngine;

namespace Enemy
{
    public class Perception
    {
        private FireRate _fireRate;
        private EyeSensor _eyeSensor;
        private HitPoint _hitPoint;
        private OverwriteOrder _overwriteOrder;

        public Perception(RequiredRef requiredRef)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; }

        /// <summary>
        /// 初期化。Startのタイミングで呼ぶ想定。
        /// </summary>
        public void InitializeOnStart()
        {
            BlackBoard bb = Ref.BlackBoard;

            // 自身のエリア
            Vector3 p = Ref.Transform.position;
            bb.Area = AreaCalculator.CreateArea(p);

            // プレイヤーのエリア
            bb.PlayerArea = AreaCalculator.CreatePlayerArea(Ref.Player);

            // スロット
            SlotSettings slot = Ref.EnemyParams.Slot;
            bb.Slot = AreaCalculator.CreateSlot(Ref.Player, slot);

            // 生存時間
            bb.LifeTime = Ref.EnemyParams.LifeTime;

            // 攻撃間隔、視界、体力、命令
            _fireRate = new FireRate(Ref);
            _eyeSensor = new EyeSensor(Ref);
            _hitPoint = new HitPoint(Ref);
            _overwriteOrder = new OverwriteOrder(bb.Name);
        }

        /// <summary>
        /// 値を更新。
        /// </summary>
        public void Update()
        {
            Calculate();
            Overwrite();
        }

        // 必要な値を計算し、黒板に書き込む。
        private void Calculate()
        {
            BlackBoard bb = Ref.BlackBoard;

            bool isHide = bb.CurrentState == StateKey.Hide;
            if (isHide) return;

            // エリアとスロットの位置を更新。
            bb.Area.Point = AreaCalculator.AreaPoint(Ref.Transform);
            bb.PlayerArea.Point = AreaCalculator.AreaPoint(Ref.Player);
            bb.Slot.Point = AreaCalculator.SlotPoint(Ref.Player, Ref.EnemyParams.Slot);

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (bb.Area.Collision(bb.PlayerArea))
            {
                bb.Area.Point = bb.Area.TouchPoint(bb.PlayerArea);
            }

            // 自身からプレイヤーへのベクトルを黒板に書き込む。
            Vector3 pv = Ref.Player.position - Ref.Transform.position;
            float pMag = pv.magnitude;
            bb.PlayerDistance = pMag;
            bb.PlayerDirection = pMag > 1E-05F ? pv / pMag : Vector3.zero;

            // プレイヤーを検知した状態ならば生存時間を減らす。
            if (bb.IsPlayerDetect) bb.LifeTime -= bb.PausableDeltaTime;

            // 自身のエリアからスロットへのベクトルを黒板に書き込む。
            Vector3 sv = bb.Slot.Point - bb.Area.Point;
            bb.SlotDirection = sv.normalized;
            bb.SlotSqrDistance = sv.sqrMagnitude;

            // 攻撃タイミングを更新。
            _fireRate.UpdateIfAttacked();

            // 視界を更新。
            _eyeSensor.Update();

            // 体力を更新。
            _hitPoint.Update();
        }

        // 黒板の値を外部からの命令で上書きする。
        private void Overwrite()
        {
            BlackBoard bb = Ref.BlackBoard;

            bool isDelete = bb.CurrentState == StateKey.Delete;
            if (isDelete) return;

            foreach (EnemyOrder order in _overwriteOrder.ForEach())
            {
                EnemyOrder.Type t = order.OrderType;

                if (t == EnemyOrder.Type.Spawn) { PlayerDetect(); }
                else if (t == EnemyOrder.Type.Attack) { AttackTrigger(); }
                else if (t == EnemyOrder.Type.Pause) { Pause(true); }
                else if (t == EnemyOrder.Type.Resume) { Pause(false); }
                else if (t == EnemyOrder.Type.BossStart) { Die(); }
                else if (t == EnemyOrder.Type.QteStartTargeted) { Qte(run: true, tgt: true); AttackTrigger(); }
                else if (t == EnemyOrder.Type.QteStartUntargeted) { Qte(run: true, tgt: false); }
                else if (t == EnemyOrder.Type.QteSuccessTargeted) { Qte(run: false, tgt: false); Die(); }
                else if (t == EnemyOrder.Type.QteSuccessUntargeted) { Qte(run: false, tgt: false); }
                else if (t == EnemyOrder.Type.QteFailureTargeted) { Qte(run: false, tgt: false); }
                else if (t == EnemyOrder.Type.QteFailureUntargeted) { Qte(run: false, tgt: false); }
            }

            void PlayerDetect() { bb.IsPlayerDetect = true; }
            void AttackTrigger() { bb.OrderedAttack.Order(); }
            void Pause(bool b) { bb.IsPause = b; }
            void Die() { _hitPoint.Damage(int.MaxValue / 2, ""); }
            void Qte(bool run, bool tgt) { bb.IsQteRunning = run; bb.IsQteTargeted = tgt; }
        }

        /// <summary>
        /// ダメージ処理。
        /// </summary>
        public void Damage(int value, string weapon) => _hitPoint.Damage(value, weapon);

        /// <summary>
        /// EnemyManagerからの命令。
        /// </summary>
        public void Order(EnemyOrder order) => _overwriteOrder.Buffer(order);

        /// <summary>
        /// EnemyManager以外から呼び出し。
        /// </summary>
        public void Order(EnemyOrder.Type type) => _overwriteOrder.Buffer(type);

        /// <summary>
        /// 描画。
        /// </summary>
        public void Draw()
        {
            Ref.BlackBoard.PlayerArea?.Draw();
            Ref.BlackBoard.Area?.Draw();
            Ref.BlackBoard.Slot?.Draw();

            _eyeSensor?.Draw();
        }
    }
}