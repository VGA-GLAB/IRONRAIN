using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class QTEBossAnimationSequence : ISequence
    {
        private enum QTEBossAnimType
        {
            MoveBossToPlayerFront,
            BreakLeftArm,
            QteCombatReady,
            FirstQteCombatAction,
            SecondQteCombatAction,
            PenetrateBoss
        }

        [OpenScriptButton(typeof(QTEBossAnimationSequence))]
        [Description("BossのQTEのAnimationを呼び出すSequence")]
        [SerializeField, Header("BossのAnimationの種類")]
        private QTEBossAnimType _qteBossAnimType;

        private EnemyManager _enemyManager;

        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // 指定されたAnimationを再生させる
            switch (_qteBossAnimType)
            {
                case QTEBossAnimType.MoveBossToPlayerFront:
                {
                    await _enemyManager.MoveBossToPlayerFrontAsync(ct);
                    break;
                }
                case QTEBossAnimType.BreakLeftArm:
                {
                    _enemyManager.BreakLeftArm();
                    await UniTask.CompletedTask;
                    break;
                }
                case QTEBossAnimType.QteCombatReady:
                {
                    _enemyManager.QteCombatReady();
                    await UniTask.CompletedTask;
                    break;
                }
                case QTEBossAnimType.FirstQteCombatAction:
                {
                    _enemyManager.FirstQteCombatAction();
                    await UniTask.CompletedTask;
                    break;
                }
                case QTEBossAnimType.SecondQteCombatAction:
                {
                    _enemyManager.SecondQteCombatAction();
                    await UniTask.CompletedTask;
                    break;
                }
                case QTEBossAnimType.PenetrateBoss:
                {
                    _enemyManager.PenetrateBoss();
                    await UniTask.CompletedTask;
                    break;
                }
            }
        }

        public void Skip()
        {
        }
    }
}