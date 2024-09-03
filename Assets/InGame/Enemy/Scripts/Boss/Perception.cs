using UnityEngine;

namespace Enemy.Boss
{
    public class Perception
    {
        private FireRate _fireRate;
        private HitPoint _hitPoint;
        private OverwriteOrder _overwriteOrder;

        public Perception(RequiredRef requiredRef)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        /// <summary>
        /// 初期化。Startのタイミングで呼ぶ想定。
        /// </summary>
        public void InitializeOnStart()
        {
            // 自身のエリア
            Vector3 p = Ref.Transform.position;
            Ref.BlackBoard.Area = AreaCalculator.CreateBossArea(p);

            // プレイヤーのエリア
            Ref.BlackBoard.PlayerArea = AreaCalculator.CreatePlayerArea(Ref.Player);

            // 攻撃間隔、体力、命令
            _fireRate = new FireRate(Ref);
            _hitPoint = new HitPoint(Ref);
            _overwriteOrder = new OverwriteOrder(Ref.BlackBoard.Name);
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

            // エリアの位置を更新
            bb.Area.Point = AreaCalculator.AreaPoint(Ref.Transform);
            bb.PlayerArea.Point = AreaCalculator.AreaPoint(Ref.Player);

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (bb.Area.Collision(bb.PlayerArea))
            {
                bb.Area.Point = bb.Area.TouchPoint(bb.PlayerArea);
            }

            if (Ref.Field != null)
            {
                // 自身から点Pへのベクトルを黒板に書き込む。
                //Vector3 ppv = Ref.PointP.transform.position - Ref.Transform.position;
                //float ppMag = ppv.magnitude;
                //bb.PointPDistance = ppMag;
                //bb.PointPDirection = ppMag > 1E-05F ? ppv / ppMag : Vector3.zero;
            }

            // 自身からプレイヤーへのベクトルを黒板に書き込む。
            Vector3 pv = Ref.Player.position - Ref.Transform.position;
            bb.PlayerDirection = pv.normalized;
            bb.PlayerSqrDistance = pv.sqrMagnitude;

            // プレイヤーが近接攻撃が届く範囲にいるか。
            Vector3 pp = Ref.Player.position;
            bb.IsWithinMeleeRange = Ref.MeleeEquip.IsWithinRange(pp);

            // ボス戦開始からの経過時間を更新。
            if (bb.IsBossStarted) bb.ElapsedTime += Time.deltaTime;

            // 仕様が決まっていないのでとりあえずの処理として、ボス戦開始から1秒後に登場完了とする。
            if (bb.ElapsedTime > 1.0f) bb.IsAppearCompleted = true;

            // 攻撃タイミングを更新。
            _fireRate.UpdateIfAttacked();

            // 体力を更新。
            _hitPoint.Update();
        }

        // 黒板の値を外部からの命令で上書きする。
        private void Overwrite()
        {
            BlackBoard bb = Ref.BlackBoard;

            bool isDelete = bb.CurrentState == StateKey.Defeated;
            if (isDelete) return;

            foreach (EnemyOrder order in _overwriteOrder.ForEach())
            {
                EnemyOrder.Type t = order.OrderType;

                if (t == EnemyOrder.Type.BossStart) { BossStart(); }
                else if (t == EnemyOrder.Type.FunnelExpand) { FunnelExpand(); }
                else if (t == EnemyOrder.Type.ResumeBossAction) { ResumeBossAction(); }
                else if (t == EnemyOrder.Type.FunnelLaserSight) { FunnelLaserSight(); }
                else if (t == EnemyOrder.Type.MoveToPlayerFront) { QteStart(); }
                else if (t == EnemyOrder.Type.BreakLeftArm) { QteStart(); BreakLeftArm(); }
                else if (t == EnemyOrder.Type.QteCombatReady) { QteStart(); QteCombatReady(); }
                else if (t == EnemyOrder.Type.FirstQteCombatAction) { QteStart(); FirstCombat(); }
                else if (t == EnemyOrder.Type.SecondQteCombatAction) { QteStart(); SecondCombat(); }
                else if (t == EnemyOrder.Type.PenetrateBoss) { QteStart(); PenetrateBoss(); }
            }

            // 黒板に書き込む命令一覧。
            void BossStart() { bb.IsBossStarted = true; }
            void FunnelExpand() { bb.FunnelExpand.Order(); }
            void ResumeBossAction() { bb.ResumeBossAction.Order(); }
            void FunnelLaserSight() { bb.IsFunnelLaserSight = true; }
            void QteStart() { bb.IsQteStarted = true; }
            void BreakLeftArm() { bb.IsBreakLeftArm = true; }
            void QteCombatReady() { bb.IsQteCombatReady = true; }
            void FirstCombat() { bb.IsFirstCombatInputed = true; }
            void SecondCombat() { bb.IsSecondCombatInputed = true; }
            void PenetrateBoss() { bb.IsPenetrateInputed = true; }
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
        }
    }
}
