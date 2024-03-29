using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 行動をオブジェクトに反映してキャラクターを動かす。
    /// </summary>
    public class Action : LifeCycle
    {
        private BlackBoard _blackBoard;
        private EnemyParams _params;
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
            EnemyParams enemyParams, Effect[] effects, IWeapon weapon)
        {
            _blackBoard = blackBoard;
            _params = enemyParams;
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

            // 行動の先頭に死亡が入っていないかチェック
            if (_blackBoard.ActionOptions.TryPeek(out ActionPlan p) && p.Choice != Choice.Broken)
            {
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
            }

            // 各アニメーションの再生時間を計算
            _animation.PlayTime();

            // 移動と回転以外の行動を実行
            while (_blackBoard.ActionOptions.Count > 0)
            {
                ActionPlan plan = _blackBoard.ActionOptions.Dequeue();

                // 撤退
                if (plan.Choice == Choice.Escape)
                {
                    // 1度この分岐に入ったら以降は入らない様なフラグが必要。
                    // アニメーション再生(移動のアニメーションと同じ？)
                    // 指定箇所まで移動？一定時間上もしくは下に移動？
                    //  ビヘイビアツリーで移動量とか決める。
                    // 消す。
                }

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
                    if (_weapon != null)
                    {
                        _weapon.Attack();
                        _blackBoard.LastAttackTime = Time.time;
                    }
                }

                // 死亡以下の優先度(キューの追加順)の行動はすべてキャンセルされる。
                if (plan.Choice == Choice.Broken) break;
            }

            // 体力が一定以下の場合は瀕死の演出を再生
            if (_blackBoard.IsDying) _effector.Play(EffectKey.Dying);

            return Result.Running;
        }

        public override void OnPreCleanup()
        {
            // 残りの生存時間から死ぬまでの生存を計算
            float lt = _params.Tactical.LifeTime - _blackBoard.LifeTime;
            CombatDesigner.ExitReport(lt, isDead: _blackBoard.Hp <= 0);

            _animation.Cleaningup();
        }
    }
}