using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 戦闘中の各行動を、更に動作単位で分けて管理する。
    /// </summary>
    public abstract class BattleActionStep
    {
        private bool _isEnter = true;

        protected abstract void Enter();
        protected abstract void Stay();

        /// <summary>
        /// 最初の1回はEnterが呼ばれ、以降はStayが呼ばれる。
        /// </summary>
        public void Update()
        {
            if (_isEnter)
            {
                _isEnter = false;
                Enter();
            }
            else
            {
                Stay();
            }
        }
    }

    /// <summary>
    /// 戦闘中の各行動をステートで管理するための基底クラス。
    /// Stayで呼ぶ前提のメソッドのみを持ち、呼び出し自体は行わない。
    /// </summary>
    public class BattleState : State
    {
        protected BossParams _params;
        protected BlackBoard _blackBoard;
        protected DebugPointP _pointP;
        protected Body _body;
        protected BodyAnimation _animation;
        protected IReadOnlyCollection<FunnelController> _funnels;

        public BattleState(StateRequiredRef requiredRef) : base(requiredRef.States)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _pointP = requiredRef.PointP;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _funnels = requiredRef.Funnels;
        }

        protected override void Enter() { }
        protected override void Exit() { }
        protected override void Stay() { }

        /// <summary>
        /// ダメージを受けた場合に音を再生。
        /// </summary>
        protected void PlayDamageSE()
        {
            string seName = "";
            if (_blackBoard.DamageSource == Const.PlayerRifleWeaponName) seName = "SE_Damage_02";
            else if (_blackBoard.DamageSource == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";

            if (seName != "") AudioWrapper.PlaySE(seName);
        }

        /// <summary>
        /// ファンネル展開
        /// </summary>
        protected void FunnelExpand()
        {
            // EnterやExitのタイミングでトリガーされた場合展開されないのでは？
            if (_blackBoard.FunnelExpandTrigger)
            {
                foreach (FunnelController f in _funnels) f.Expand();
                AudioWrapper.PlaySE("SE_Funnel");
            }
        }

        /// <summary>
        /// ファンネルのレーザーサイトを表示
        /// </summary>
        protected void FunnelLaserSight()
        {
            if (_blackBoard.IsFunnelLaserSight)
            {
                foreach (FunnelController f in _funnels) f.LaserSight(true);
            }
        }

        /// <summary>
        /// 点Pに向けて移動。
        /// </summary>
        protected void MoveToPointP()
        {
            Vector3 dir = _blackBoard.PointPDirection;
            Vector3 mpf = dir * _params.MoveSpeed.Chase * _blackBoard.PausableDeltaTime;
            if (mpf.magnitude >= _blackBoard.PointPDistance)
            {
                _body.Warp(_pointP.transform.position);
            }
            else
            {
                _body.Warp(_blackBoard.Area.Point + mpf);
            }
        }

        /// <summary>
        /// プレイヤーを向く。
        /// </summary>
        protected void LookAtPlayer()
        {
            Vector3 look = _blackBoard.PlayerDirection;
            look.y = 0;
            _body.Forward(look);
        }
    }
}