using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class ResetAnimationSpeedSequence : ISequence
    {
        private PlayerAnimation _playerAnimation;
        
        public void SetData(SequenceData data)
        {
            _playerAnimation = data.PlayerAnimation;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _playerAnimation.AnimationSpeedReset();
            
            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _playerAnimation.AnimationSpeedReset();
        }
    }
}
