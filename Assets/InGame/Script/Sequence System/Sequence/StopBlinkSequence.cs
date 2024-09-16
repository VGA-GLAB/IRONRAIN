using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class StopBlinkSequence : ISequence
    {
        [OpenScriptButton(typeof(StopBlinkSequence))]
        [Description("スタートした点滅を止めるSequence")]
        [Header("対象"), SerializeField]
        private CockpitEmissionController.EmissionTargetType _target;

        private CockpitEmissionController _cockpitEmissionController;
        
        public void SetData(SequenceData data)
        {
            _cockpitEmissionController = data.CockpitEmissionController;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _cockpitEmissionController.StopBlink(_target);

            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _cockpitEmissionController.StopBlink(_target);
        }
    }
}
