using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ApproachStateと雑魚3種それぞれのBattleStateの基底クラス。
    /// 登場させてから死ぬまでに共通して使うメソッド群。
    /// </summary>
    public abstract class PlayableState : State<StateKey>
    {
        public PlayableState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; }

        protected override void Enter() { }
        protected override void Exit() { }
        protected override void Stay() { }

        /// <summary>
        /// ダメージを受けた場合に音を再生。
        /// </summary>
        protected void PlayDamageSE()
        {
            string source = Ref.BlackBoard.DamageSource;
            Vector3 p = Ref.Body.Position;
            DamageSE.PlayEnemy(p, source);
        }

        /// <summary>
        /// 移動した方向ベクトルで下半身のアニメーションを制御。
        /// z軸を前方向として、ベクトルのx成分の正負で左右どちらに移動したかを判定する。
        /// </summary>
        protected void LeftRightMoveAnimation(in Vector3 moveDir)
        {
            // 移動量に対する倍率。
            const float Mag = 2.0f; 

            float value = Mathf.Clamp(moveDir.x * Mag, -1.0f, 1.0f);
            LeftRightMoveAnimation(value);
        }

        /// <summary>
        /// 左右移動のアニメーションを制御。
        /// </summary>
        public void LeftRightMoveAnimation(float value)
        {
            string param = Const.Param.SpeedX;
            Ref.BodyAnimation.SetFloat(param, value);
        }

        /// <summary>
        /// 前後移動のアニメーションを制御。
        /// </summary>
        public void ForwardBackMoveAnimation(float value)
        {
            string param = Const.Param.SpeedZ;
            Ref.BodyAnimation.SetFloat(param, value);
        }

        /// <summary>
        /// 死亡もしくは撤退の場合は、アイドル状態を経由して退場するステートに遷移。
        /// </summary>
        protected bool ExitIfDeadOrTimeOver()
        {
            if (Ref.BlackBoard.Hp <= 0) { TryChangeState(StateKey.Broken); return true; }
            else if (Ref.BlackBoard.LifeTime <= 0) { TryChangeState(StateKey.Escape); return true; }

            return false;
        }

        /// <summary>
        /// 黒板に撃破された座標を書き込む。
        /// </summary>
        protected void WriteBrokenPosition()
        {
            Ref.BlackBoard.BrokenPosition = Ref.Body.Position;
        }

        /// <summary>
        /// スラスターとジェットパックの位置の更新。
        /// </summary>
        protected void UpdateSePosition()
        {
            Vector3 p = Ref.Body.Position;
            int thruster = Ref.BlackBoard.ThrusterSE;
            int jet = Ref.BlackBoard.JetSE;
            AudioWrapper.UpdateSePosition(p, thruster);
            AudioWrapper.UpdateSePosition(p, jet);
        }
    }
}
