using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SoundStopSequence : ISequence
    {
        [OpenScriptButton(typeof(SoundStopSequence))]
        [Description("指定したIDで流した音をとめるSequence")]
        [Header("止める音のID"), SerializeField]
        private int _id = -1;

        private SequenceData.SoundSequenceManager _soundSequenceManager;
        
        public void SetData(SequenceData data)
        {
            _soundSequenceManager = data.SoundManager;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            var index = _soundSequenceManager.UnregisterIndex(_id);
            
            CriAudioManager.Instance.SE.Stop(index);
            
            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            var index = _soundSequenceManager.UnregisterIndex(_id);
            
            CriAudioManager.Instance.SE.Stop(index);
        }
    }
}
