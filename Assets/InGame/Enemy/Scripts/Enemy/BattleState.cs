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
        // ホバリングさせるための値。
        private float _hovering;
        // 左右移動のアニメーションのパラメータ。
        private float _blend;
        private int _sign;


        public BattleState(RequiredRef requiredRef) : base(requiredRef)
        {
        }

        protected sealed override void Enter() 
        {
            _lerp = 0;
            _startX = Ref.Body.Position.x;
            _endX = Ref.BlackBoard.Slot.Point.x;
            _hovering = 0;

            OnEnter();
            Always();
        }
        protected sealed override void Exit()
        {
            Always();
            OnExit();
            WritePlayerRelativePosition();
        }      
        protected sealed override void Stay() 
        {
            Always();

            if (ExitIfDeadOrTimeOver()) return;

            StayIfBattle();
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();

        /// <summary>
        /// ダメージ音、死亡または時間切れで遷移、アニメーション付きの移動。
        /// これらが処理された後、装備している武器毎に動きが違う。
        /// </summary>
        protected abstract void StayIfBattle();

        private void Always()
        {
            PlayDamageSE();

            // ホバリングで上下に動かす。
            float h = Mathf.Sin(_hovering);
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _hovering += dt;
            Ref.Body.OffsetWarp(Vector3.up * h);

            const float Duration = 1.0f;
            // 一定間隔で自身の位置とスロットの位置を比較して次の移動位置を決める。
            _lerp += dt;
            if (_lerp >= Duration)
            {
                Vector3 p = Ref.BlackBoard.Slot.Point;
                p.x = _endX;
                Ref.Body.Warp(p);

                _lerp = 0;
                _startX = Ref.Body.Position.x;
                _endX = Ref.BlackBoard.Slot.Point.x;

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

            {
                // アイドルから左右移動のアニメーションに切り替わる速さ。
                const float Speed = 5.0f;
                // _signには移動しない場合は0、左右に移動する場合は-1もしくは1が代入されている。
                // ブレンドツリーのパラメータをその値に徐々に変化させる。
                _blend = Mathf.Clamp(_blend, -1, 1);
                _blend = Mathf.MoveTowards(_blend, _sign, dt * Speed);
                //LeftRightMoveAnimation(_blend);
                LeftRightMoveAnimation(0);
            }

            {
                // 良い感じになる前後移動のアニメーションのブレンド値。
                const float BlendTarget = 0.3f;
                const float Speed = 5.0f;
                // 前後移動のアニメーションが銃口の上下の向きと連動しているので、プレイヤーを向くよう修正。
                string param = Const.Param.SpeedZ;
                float current = Ref.BodyAnimation.GetFloat(param);
                current = Mathf.MoveTowards(current, BlendTarget, dt * Speed);
                ForwardBackMoveAnimation(current);
            }
        }
    }
}
