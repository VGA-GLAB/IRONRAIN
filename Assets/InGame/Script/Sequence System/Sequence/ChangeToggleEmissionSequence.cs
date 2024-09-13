using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class ChangeToggleEmissionSequence : ISequence
    {
        
        [OpenScriptButton(typeof(ChangeToggleEmissionSequence))]
        [Description("Toggleの光らせる色を変えるSequenceです。")]
        [SerializeField, Header("このSequenceを抜けるまでの時間")]
        private float _totalSec = 0F;
        [SerializeField, Header("対象のToggle"), Range(1, 6)]
        private int _targetToggle = 1;
        [SerializeField, Header("前のEmission"), ColorUsage(false,true)]
        private Color _beforeColor = Color.black;
        [SerializeField, Header("後のEmission"), ColorUsage(false,true)]
        private Color _afterColor = Color.magenta;
        [SerializeField, Header("色を変更していく秒数")]
        private float _duration = 1F;

        private ToggleButton[] _toggleButtons;
        
        public void SetData(SequenceData data)
        {
            _toggleButtons = data.ToggleButtons;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            var target = _toggleButtons[_targetToggle - 1];
            
            DOTween.To(() => _beforeColor, x => target.SetEmission(x), _afterColor, _duration)
                .ToUniTask(cancellationToken: ct)
                .Forget();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _toggleButtons[_targetToggle - 1].SetEmission(_afterColor);
        }
    }
}
