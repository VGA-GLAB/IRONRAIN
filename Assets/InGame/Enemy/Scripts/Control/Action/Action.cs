using System.Collections.Generic;
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
        private Body _body;
        private BodyAnimation _animation;
        private Effector _effector;
        private StateMachine _stateMachine;

        public Action(Transform transform, Transform offset, Transform rotate, Animator animator, 
            Renderer[] renderers, AnimationEvent animationEvent, BlackBoard blackBoard,
            EnemyParams enemyParams, Effect[] effects, IEquipment equipment)
        {
            _blackBoard = blackBoard;
            _params = enemyParams;
            _body = new Body(transform, offset, rotate, renderers);
            _animation = new BodyAnimation(animator, animationEvent);
            _effector = new Effector(effects);
            _stateMachine = new StateMachine(blackBoard, _body, _animation, _effector, equipment);
        }

        public override Result UpdateEvent()
        {
            _animation.PlaySpeed(ProvidePlayerInformation.TimeScale);
            _stateMachine.Update();

            return Result.Running;
        }

        public override void OnPreCleanup()
        {
            // 残りの生存時間から死ぬまでの生存を計算
            float lt = _params.Battle.LifeTime - _blackBoard.LifeTime;
            CombatDesigner.ExitReport(lt, isDead: _blackBoard.Hp <= 0);

            _animation.Cleaningup();
            _stateMachine.Destroy();
        }

        public override void OnDestroyEvent()
        {
            // 死亡もしくは撤退前にゲームが終了した場合に後始末がされないのを防ぐ
            OnPreCleanup();
        }
    }
}