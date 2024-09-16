using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class StartBlinkSequence : ISequence
    {
        [OpenScriptButton(typeof(StartBlinkSequence))]
        [Description("ボタンの点滅を開始するSequence")]
        [Header("対象"), SerializeField]
        private CockpitEmissionController.EmissionTargetType _target;
        [Header("光らせる前の色"), SerializeField]
        private Color _beforeColor = Color.black;
        [Header("光らせた後の色"), SerializeField]
        private Color _afterColor = Color.magenta;
        [Header("点滅させる時間")]
        private float _duration = 1F;
        
        private CockpitEmissionController _cockpitEmissionController;
        
        public void SetData(SequenceData data)
        {
            _cockpitEmissionController = data.CockpitEmissionController;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _cockpitEmissionController.StartBlink(_target, _beforeColor, _afterColor, _duration);
            
            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _cockpitEmissionController.StartBlink(_target, _beforeColor, _afterColor, _duration);
        }
    }
}
