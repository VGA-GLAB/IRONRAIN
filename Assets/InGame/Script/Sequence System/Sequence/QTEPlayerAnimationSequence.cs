using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class QTEPlayerAnimationSequence : ISequence
    {
        private enum AnimationType
        {
            BreakLeftArm,
            Guard,
            Finish
        }

        [OpenScriptButton(typeof(QTEPlayerAnimationSequence))]
        [Description("BossQTEでのPlayerのAnimationを呼び出すSequence")]
        [SerializeField, Header("PlayerのQTEのAnimation")]
        private AnimationType _animationType;

        [SerializeField, Header("PlayerのAnimationを待機するのか")]
        private bool _isAwait = false;
        
        private PlayerAnimation _playerAnimation;
        
        public void SetData(SequenceData data)
        {
            _playerAnimation = data.PlayerAnimation;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            UniTask animUniTask = UniTask.CompletedTask;

            switch (_animationType)
            {
                case AnimationType.BreakLeftArm:
                {
                    animUniTask = _playerAnimation.LeftArmDestroy(ct);
                    CriAudioManager.Instance.SE.Play("SE", "SE_Alert");
                    break;
                }
                case AnimationType.Guard:
                {
                    animUniTask = _playerAnimation.QteGuard(ct);
                    break;
                }
                case AnimationType.Finish:
                {
                    animUniTask = _playerAnimation.QteFinish(ct);
                    break;
                }
            }
            
            // 待機するかどうか
            if (_isAwait)
            {
                await animUniTask;
            }
            else
            {
                animUniTask.Forget(exceptionHandler);

                await UniTask.CompletedTask;
            }
        }

        public void Skip() { }
    }
}
