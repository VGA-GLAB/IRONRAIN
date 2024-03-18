using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 行動をオブジェクトに反映してキャラクターを動かす。
    /// </summary>
    public class Action : LifeCycle
    {
        private BlackBoard _blackBoard;
        private BodyMove _move;
        private OffsetMove _offsetMove;
        private BodyRotate _rotate;
        private BodyAnimation _animation;
        private Effector _effector;
        private IWeapon _weapon;

        // 死亡もしくは退場のアニメーションもしくはが終了し、非表示になったフラグ。
        // このフラグが立った次のフレーム以降はこのクラスの処理を行わない。
        private bool _isDisable;

        public Action(Transform transform, Transform offset, Transform rotate, Animator animator, BlackBoard blackBoard,
            Effect[] effects, IWeapon weapon)
        {
            _blackBoard = blackBoard;
            _move = new BodyMove(transform);
            _offsetMove = new OffsetMove(offset);
            _rotate = new BodyRotate(rotate);
            _animation = new BodyAnimation(animator);
            _effector = new Effector(effects);

            if (weapon == null) Debug.LogWarning($"武器無し: {transform.name}");
            else _weapon = weapon;

            _isDisable = false;
        }

        public override Result UpdateEvent()
        {
            // 死亡もしくは退場済みなので完了を返す。
            if (_isDisable) return Result.Complete;

            // deltaTimeぶんの移動を上書きする恐れがあるので、座標を直接書き換える処理を先にしておく。
            while (_blackBoard.WarpOptions.Count > 0)
            {
                WarpPlan plan = _blackBoard.WarpOptions.Dequeue();
                _move.Warp(plan.Position);
            }

            // deltaTimeぶんの移動
            while (_blackBoard.MovementOptions.Count > 0)
            {
                MovementPlan plan = _blackBoard.MovementOptions.Dequeue();
                _move.Move(plan.Direction * plan.Speed);
            }

            // 前方向を変更
            while (_blackBoard.ForwardOptions.Count > 0)
            {
                ForwardPlan plan = _blackBoard.ForwardOptions.Dequeue();
                _rotate.Forward(plan.Value);
            }

            // 各アニメーションの再生時間を計算
            _animation.PlayTime();

            // 移動と回転以外の行動を実行
            while (_blackBoard.ActionOptions.Count > 0)
            {
                ActionPlan plan = _blackBoard.ActionOptions.Dequeue();

                // 死亡
                // 他のアニメーションが再生されていても強制的に死亡アニメーションを再生
                if (plan.Choice == Choice.Broken && !_animation.IsPlaying(AnimationKey.Broken))
                {
                    // 再生終了後のコールバックで非表示にするフラグを立てる。
                    // コールバックが呼び出され、次のこのメソッドの呼び出し時は何も処理をせず完了を返す。
                    _animation.Play(AnimationKey.Broken, () => _isDisable = true);
                }

                // 攻撃
                if (plan.Choice == Choice.Attack)
                {
                    // 死亡もしくは攻撃アニメーションが再生中でなければアニメーションを再生                    
                    if (!_animation.IsPlaying(AnimationKey.Broken) && !_animation.IsPlaying(AnimationKey.Attack))
                    {
                        _animation.Play(AnimationKey.Attack);
                    }

                    // 武器毎の攻撃処理
                    // アニメーションの任意のタイミングで攻撃判定が未実装。
                    if (_weapon != null) _weapon.Attack();
                }

                // 死亡以下の優先度(キューの追加順)の行動はすべてキャンセルされる。
                if (plan.Choice == Choice.Broken) break;
            }

            // 体力が一定以下の場合は瀕死の演出を再生
            if (_blackBoard.IsDying) _effector.Play(EffectKey.Dying);

            return Result.Running;
        }

        public override Result LateUpdateEvent()
        {
            // このフレームで行う行動が完了したので全て削除
            _blackBoard.ActionOptions.Clear();
            _blackBoard.WarpOptions.Clear();
            _blackBoard.MovementOptions.Clear();
            _blackBoard.ForwardOptions.Clear();

            return Result.Running;
        }

        public override void OnPreCleanup()
        {
            _animation.Cleaningup();
        }
    }
}