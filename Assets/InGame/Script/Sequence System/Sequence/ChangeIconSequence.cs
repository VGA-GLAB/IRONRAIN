using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class ChangeIconSequence : ISequence
    {
        [OpenScriptButton(typeof(ChangeIconSequence))]
        [Description("説明用のアイコン操作するSequence")]
        [Header("やりたい操作"), SerializeField]
        private IndicationUIType _actionType;

        private RadarMap _radarMap;
        
        public void SetData(SequenceData data)
        {
            _radarMap = data.RadarMap;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _radarMap._indicationPanelController.ChangeIndicationUI(_actionType);
            
            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _radarMap._indicationPanelController.ChangeIndicationUI(_actionType);
        }
    }
}
