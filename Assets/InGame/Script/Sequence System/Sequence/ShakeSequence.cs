using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class ShakeSequence : ISequence
    {
        [OpenScriptButton(typeof(ShakeSequence))]
        [Description("揺れを起こすSequence")]
        [SerializeField, Header("揺れる秒数")]
        private float _shakeDuration = 1F;
        [SerializeField, Header("揺れの強さ")]
        private float _shakeStrength = 0.1F;
        [SerializeField, Header("次のSequenceに進む際揺れの終了を待つかどうか")]
        private bool _isWaitShake;
        
        private Camera _mainCamera;

        private Tween _shakeTween = null;
        
        public void SetData(SequenceData data)
        {
            _mainCamera = data.MainCamera;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // 揺らす
            UniTask shakeUniTask = UniTask.CompletedTask;
            
            if (_shakeTween is null)
            {
                _shakeTween = _mainCamera.DOShakePosition(_shakeDuration, _shakeStrength)
                    .OnComplete(() => _shakeTween = null);
                shakeUniTask = _shakeTween.ToUniTask(cancellationToken: ct);
            }

            // 揺れの終了を待つかどうか
            await (_isWaitShake ? shakeUniTask : UniTask.CompletedTask);
        }

        public void Skip() { }
    }
}
