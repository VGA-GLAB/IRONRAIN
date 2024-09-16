using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class HangerUIAnimationSequence : AbstractWaitSequence
    {
        private enum AnimType
        {
            StartActive,
            ButtonActive,
            WaitLaunch
        }

        [OpenScriptButton(typeof(HangerUIAnimationSequence))]
        [Description("UIのAnimationを呼び出すSequence")]
        [SerializeField, Header("Animationのタイプ")]
        private AnimType _animType;
        
        private LaunchManager _launchManager;
        
        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _launchManager = data.LaunchManager;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);

            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            switch (_animType)
            {
                case AnimType.StartActive:
                {
                    await _launchManager.StartActivateUi(ct);
                    break;
                }
                case AnimType.ButtonActive:
                {
                    _launchManager.ButtonActive();
                    await UniTask.CompletedTask;
                    break;
                }
                case AnimType.WaitLaunch:
                {
                    await _launchManager.WaitLaunchAnimation(ct);
                    break;
                }
                default:
                {
                    await UniTask.CompletedTask;
                    break;
                }
            }
            
            loopCts.Cancel();
        }

        public override void Skip()
        {
            switch (_animType)
            {
                case AnimType.StartActive:
                {
                    _launchManager.SkipStartActive();
                    break;
                }
                case AnimType.ButtonActive:
                {
                    _launchManager.SkipButtonActive();
                    break;
                }
                case AnimType.WaitLaunch:
                {
                    _launchManager.SkipLaunchAnimation();
                    break;
                }
            }
        }
    }
}
