using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 雑魚3種それぞれのBattleStateの基底クラス。
    /// </summary>
    public abstract class BattleState : PlayableState
    {
        // 一定間隔でスロットの位置を検知し、Lerpで移動。
        private float _lerp;
        private float _startX;
        private float _endX;
        // 全員同じ移動間隔だとおかしいので、ランダム性を持たせる。
        private float _randomDelay;
        // ホバリングさせるための値。
        private float _hovering;
        // 左右移動のアニメーションのパラメータ。
        private float _blend;
        private int _sign;

        public BattleState(RequiredRef requiredRef) : base(requiredRef) { }

        protected sealed override void Enter() 
        {
            Ref.BlackBoard.CurrentState = StateKey.Battle;
            _hovering = 0;

            UpdateDestination();
            OnEnter();
        }

        protected sealed override void Exit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろす。
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackEnd);
            Ref.BodyAnimation.SetUpperBodyWeight(0);

            OnExit();
            WriteBrokenPosition();
        }
        
        protected sealed override void Stay() 
        {
            OnStay();
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();
        protected abstract void OnStay();

        // スロットの位置を基準に移動。
        protected void Move()
        {
            const float Duration = 1.0f;
            // 一定間隔で自身の位置とスロットの位置を比較して次の移動位置を決める。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt;
            if (_lerp >= Duration + _randomDelay)
            {
                Vector3 p = Ref.BlackBoard.Slot.Point;
                p.x = _endX;
                Ref.Body.Warp(p);

                UpdateDestination();

                // プレイヤーが移動していない場合はアニメーションさせない。
                // 移動開始位置と終了位置を比較し、左右に移動する場合は-1もしくは1、移動しない場合は0。
                _sign = System.Math.Sign(_startX - _endX);
            }
            else
            {
                // x軸の値をLerpで操作することで左右移動する。
                Vector3 p = Ref.BlackBoard.Slot.Point;
                p.x = Mathf.Lerp(_startX, _endX, _lerp);
                Ref.Body.Warp(p);
            }
        }

        // 左右移動のアニメーション。
        protected void LeftRightMoveAnimation()
        {
            // アイドルから左右移動のアニメーションに切り替わる速さ。
            const float Speed = 5.0f;
            // _signには移動しない場合は0、左右に移動する場合は-1もしくは1が代入されている。
            // ブレンドツリーのパラメータをその値に徐々に変化させる。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _blend = Mathf.Clamp(_blend, -1, 1);
            _blend = Mathf.MoveTowards(_blend, _sign, dt * Speed);
            LeftRightMoveAnimation(_blend);
        }

        // 前後移動のアニメーション。
        protected void ForwardBackMoveAnimation()
        {
            // 良い感じになる前後移動のアニメーションのブレンド値。
            const float BlendTarget = 0.3f;
            const float Speed = 5.0f;
            // 前後移動のアニメーションが銃口の上下の向きと連動しているので、プレイヤーを向くよう修正。
            string param = Const.Param.SpeedZ;
            float current = Ref.BodyAnimation.GetFloat(param);
            float dt = Ref.BlackBoard.PausableDeltaTime;
            current = Mathf.MoveTowards(current, BlendTarget, dt * Speed);
            ForwardBackMoveAnimation(current);
        }

        // ホバリングで上下に動かす。
        protected void Hovering()
        {
            float h = Mathf.Sin(_hovering);
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _hovering += dt;
            Ref.Body.OffsetWarp(Vector3.up * h);
        }

        // 移動先の更新。
        private void UpdateDestination()
        {
            _lerp = 0;
            _startX = Ref.Body.Position.x;
            _endX = Ref.BlackBoard.Slot.Point.x;

            // ランダム力を強くすると、敵の動き始めが遅くなるので控えめに。
            const float RandomPower = 0.5f;
            _randomDelay = Random.value * RandomPower;
        }
    }
}
