using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// アニメーション含む移動を実際にオブジェクトに適用する。
    /// </summary>
    public class MoveApply
    {
        private Choice _choice;
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;

        public MoveApply(Choice choice, BlackBoard blackBoard, Body body, BodyAnimation animation)
        {
            _choice = choice;
            _blackBoard = blackBoard;
            _body = body;
            _animation = animation;
        }

        /// <summary>
        /// 回転しつつ移動させる。
        /// ブレンドツリーの値を書き換えてアニメーションを制御。
        /// </summary>
        public void Run()
        {
            Vector3 before = _body.TransformPosition;

            // 座標を直接書き換える。
            // deltaTimeぶんの移動を上書きする恐れがあるので移動より先。
            while (_blackBoard.WarpOptions.TryDequeue(out WarpPlan plan))
            {
                if (plan.Choice != _choice) continue;

                _body.Warp(plan.Position);
            }

            // 移動
            while (_blackBoard.MovementOptions.TryDequeue(out MovementPlan plan))
            {
                if (plan.Choice != _choice) continue;

                _body.Move(plan.Direction * plan.Speed);
            }

            // 回転
            while (_blackBoard.ForwardOptions.TryDequeue(out ForwardPlan plan))
            {
                if (plan.Choice != _choice) continue;

                _body.Forward(plan.Value);
            }

            Vector3 after = _body.TransformPosition;
            
            // BlendTreeで前後左右の移動のアニメーションをブレンドする。
            // 値は -1~1 の範囲をとり、そのままだと変化量が微々たるものなのでn倍する。
            Vector3 sub = (after - before) * EnemyParams.Debug.BlendTreeParameterMag;
            _animation.SetParameter(Const.AnimationParam.LeftRight, -sub.x);
            _animation.SetParameter(Const.AnimationParam.ForwardBack, sub.z);
        }
    }
}
