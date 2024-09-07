using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class ActionState : State<StateKey>
    {
        private float _elapsed;
        private float _weight;

        public ActionState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Action;

            Ref.Callback.InvokeAttackAction();

            EnemyController target = Ref.NpcParams.Target;
            if (target != null) target.Damage(int.MaxValue / 2, "NPC");

            // 武器構え->射撃->アイドルに戻る が自動で再生される。
            string param = Const.Param.AttackSet;
            Ref.BodyAnimation.SetTrigger(param);

            _elapsed = 0;
            _weight = 0;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // UpperBodyLayerのWeightの上昇速度。
            const float WeightIncreaseSpeed = 3.0f;

            // 徐々にWeightを上げていく。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _weight += dt * WeightIncreaseSpeed;
            _weight = Mathf.Clamp01(_weight);
            Ref.BodyAnimation.SetUpperBodyWeight(_weight);

            Vector3 dir = Ref.Body.Forward;
            float spd = Ref.NpcParams.MoveSpeed;
            Vector3 velo = dir * dt * spd;
            Ref.Body.Move(velo);

            // アニメーションの再生終了を待って遷移。
            const float AnimationEnd = 3.0f;

            if (_elapsed > AnimationEnd) TryChangeState(StateKey.Escape);
            else _elapsed += dt;
        }
    }
}
