using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 雑魚3種類のStayで共通して呼ぶメソッドのみを持ち、呼び出し自体は行わない。
    /// </summary>
    public class BattleState : State
    {
        protected EnemyParams _params;
        protected BlackBoard _blackBoard;
        protected Body _body;
        protected BodyAnimation _animation;

        public BattleState(EnemyParams enemyParams, BlackBoard blackBoard, Body body, BodyAnimation animation)
        {
            _params = enemyParams;
            _blackBoard = blackBoard;
            _body = body;
            _animation = animation;
        }

        public override StateKey Key => StateKey.Battle;

        protected override void Enter() { }
        protected override void Exit() { }
        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable) { }

        // ダメージを受けた場合に音を再生。
        protected void PlayDamageSE()
        {
            string seName = "";
            if (_blackBoard.DamageSource == Const.PlayerAssaultRifleWeaponName) seName = "SE_Damage_02";
            else if (_blackBoard.DamageSource == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";
            
            if (seName != "") AudioWrapper.PlaySE(seName);
        }

        // 死亡もしくは撤退の場合は、アイドル状態を経由して退場するステートに遷移。
        protected bool BattleExit(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            bool isExit = false;
            foreach (ActionPlan plan in _blackBoard.ActionPlans)
            {
                if (plan.Choice == Choice.Broken || plan.Choice == Choice.Escape) isExit = true;
            }

            if (isExit) { TryChangeState(stateTable[StateKey.Idle]); }

            return isExit;
        }

        // アニメーションを含めた移動。
        protected void Move()
        {
            // 左右どちらに移動したかを判定する用途。
            Vector3 before = _body.TransformPosition;

            // 移動を上書きする恐れがあるので、先に座標を直接書き換える。
            while (_blackBoard.WarpPlans.TryDequeue(out ActionPlan.Warp plan))
            {
                if (plan.Choice == Choice.Chase) _body.Warp(plan.Position);
            }
            // 移動
            while (_blackBoard.MovePlans.TryDequeue(out ActionPlan.Move plan))
            {
                if (plan.Choice == Choice.Chase)
                {
                    _body.Move(plan.Direction * plan.Speed * _blackBoard.PausableDeltaTime);
                }
            }
            // 回転
            while (_blackBoard.LookPlans.TryDequeue(out ActionPlan.Look plan))
            {
                if (plan.Choice == Choice.Chase) _body.Forward(plan.Forward);
            }

            // 別レイヤーにある左右移動のアニメーションを並行して再生できない？
            {
                // 移動した方向ベクトルでアニメーションを制御。
                // z軸を前方向として、ベクトルのx成分の正負で左右どちらに移動したかを判定する。
                //bool isRightMove = _body.TransformPosition.x - before.x > 0;
                //_animation.SetBool(Const.AnimationParam.IsRightMove, isRightMove);
                //_animation.SetBool(Const.AnimationParam.IsLeftMove, !isRightMove);
            }
        }
    }
}
