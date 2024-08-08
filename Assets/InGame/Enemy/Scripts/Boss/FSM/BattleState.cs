using UnityEngine;
using System.Collections.Generic;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 登場後、戦闘するステート。
    /// </summary>
    public class BattleState : State
    {
        private enum AnimationGroup
        {
            Other,       // 初期状態
            Idle,        // Idle
            Hold,        // HoldStart~HoldLoop
            Fire,        // FireLoop
            Blade,       // BladeStart~BladeLoop
            BladeAttack, // BladeAttack
        }

        private BossParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private IReadOnlyCollection<FunnelController> _funnels;
        private AgentScript _agentScript;

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleState(StateRequiredRef requiredRef) : base(requiredRef.States)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _funnels = requiredRef.Funnels;
            _agentScript = requiredRef.AgentScript;

            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimationConst.Boss.Idle, BodyAnimationConst.Layer.BaseLayer, AnimationGroup.Idle);
            Register(BodyAnimationConst.Boss.HoldStart, BodyAnimationConst.Layer.BaseLayer, AnimationGroup.Hold);
            Register(BodyAnimationConst.Boss.FireLoop, BodyAnimationConst.Layer.BaseLayer, AnimationGroup.Fire);
            Register(BodyAnimationConst.Boss.BladeStart, BodyAnimationConst.Layer.BaseLayer, AnimationGroup.Blade);
            Register(BodyAnimationConst.Boss.BladeAttack, BodyAnimationConst.Layer.BaseLayer, AnimationGroup.BladeAttack);

            // stateNameのアニメーションのステートに遷移してきたタイミング(Enter)のみトリガーしている。
            // このメソッドで登録していないアニメーションのステートに遷移した場合、
            // _currentAnimGroupの値が元のままになるので注意。
            void Register(string stateName, int layerIndex, AnimationGroup animGroup)
            {
                //_animation.RegisterStateEnterCallback(StateKey.Battle, stateName, layerIndex, () => _currentAnimGroup = animGroup);
            }
        }

        protected override void Enter()
        {
            if (_agentScript != null) _agentScript.EnemyGenerate();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // ダメージを受けた場合に音を再生。
            string seName = "";
            if (_blackBoard.DamageSource == Const.PlayerRangeWeaponName) seName = "SE_Damage_02";
            else if (_blackBoard.DamageSource == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";
            if (seName != "") AudioWrapper.PlaySE(seName);

            // 移動前後の位置を比較して左右どちらに移動したかを判定する。
            Vector3 dir = _blackBoard.TransformToPointPDirection;
            Vector3 mpf = dir * _params.MoveSpeed.Chase * _blackBoard.PausableDeltaTime;
            if (mpf.magnitude >= _blackBoard.TransformToPointPDistance)
            {
                _body.Warp(_blackBoard.PointP);
            }
            else
            {
                _body.Warp(_blackBoard.Area.Point + mpf);
            }

            // プレイヤーの方向を向く。
            Vector3 look = _blackBoard.TransformToPlayerDirection;
            look.y = 0;
            _body.Forward(look);

            // QTEイベントが始まった場合は遷移。
            if (_blackBoard.IsQteEventStarted) { TryChangeState(StateKey.QteEvent); return; }

            // ファンネル展開。
            if (_blackBoard.FunnelExpandTrigger)
            {
                foreach (FunnelController f in _funnels) f.Expand();
                AudioWrapper.PlaySE("SE_Funnel");
            }

            // どのアニメーションが再生されているかによって処理を分ける。
            if (_currentAnimGroup == AnimationGroup.Idle) StayIdle();
            else if (_currentAnimGroup == AnimationGroup.Hold) StayHold();
            else if (_currentAnimGroup == AnimationGroup.Fire) StayFire();
            else if (_currentAnimGroup == AnimationGroup.Blade) StayBlade();
            else if (_currentAnimGroup == AnimationGroup.BladeAttack) StayBladeAttack();
            else StayOther();
        }

        public override void Dispose()
        {
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            // 近接攻撃の範囲内かつ、タイミングが来ていた場合は攻撃する。
            if (_blackBoard.IsWithinMeleeRange && _blackBoard.NextMeleeAttackTime < Time.time)
            {
                _animation.ResetTrigger(BodyAnimationConst.Param.AttackSetTrigger);
                /* _animation.SetTrigger(近接攻撃構えのトリガー名); */
            }
            // または、遠距離攻撃タイミングが来ていた場合は攻撃する。
            else if (_blackBoard.NextRangeAttackTime < Time.time)
            {
                _animation.SetTrigger(BodyAnimationConst.Param.AttackSetTrigger);
                /* _animation.ResetTrigger(近接攻撃構えのトリガー名); */
            }
        }

        // アニメーションが銃構え状態
        private void StayHold()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimationConst.Param.AttackTrigger);
        }

        // アニメーションが銃攻撃状態
        private void StayFire()
        {
            // 射撃のアニメーションのステートが繰り返されるようになっているため、
            // 手動で射撃終了をトリガーしないと近接攻撃出来ない。
            if (_blackBoard.IsWithinMeleeRange && _blackBoard.NextMeleeAttackTime < Time.time)
            {
                _animation.SetTrigger(BodyAnimationConst.Param.AttackEndTrigger);
            }
        }

        // アニメーションが刀構え状態
        private void StayBlade()
        {
            //
        }

        // アニメーションが刀攻撃状態
        private void StayBladeAttack()
        {
            //
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
            //
        }
    }
}