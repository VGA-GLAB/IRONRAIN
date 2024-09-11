using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SetAisacSequence : ISequence
    {
        [OpenScriptButton(typeof(SetAisacSequence))]
        [Description("BGMのAISACの値を変更させるSequence")]
        [SerializeField, Header("変更前の値")]
        private float _aisacBeforeValue = 0F;

        [SerializeField, Header("変更後の値")]
        private float _aisacAfterValue = 0F;
        
        [SerializeField, Header("ControlIDName")]
        private string _aisacIdName = "";

        [SerializeField, Header("Animationさせる秒数")]
        private float _animDuration = 1F;
        
        public void SetData(SequenceData data) { }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            ChangeAisacAsync(ct).Forget();
            return UniTask.CompletedTask;
        }

        private async UniTaskVoid ChangeAisacAsync(CancellationToken ct)
        {
            await DOTween.To(
                () => _aisacBeforeValue,
                x => CriAudioManager.Instance.BGM.SetAisac(_aisacIdName, x),
                _aisacAfterValue,
                _animDuration).ToUniTask(cancellationToken: ct);
        }

        public void Skip() { }
    }
}
