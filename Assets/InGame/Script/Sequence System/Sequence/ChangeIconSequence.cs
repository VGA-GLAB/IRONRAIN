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
        private IndicationUiType _actionType;

        private RaderMap _raderMap;
        
        public void SetData(SequenceData data)
        {
            _raderMap = data.RaderMap;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _raderMap.ChangeIndicationUi(_actionType);
            
            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _raderMap.ChangeIndicationUi(_actionType);
        }
    }
}
