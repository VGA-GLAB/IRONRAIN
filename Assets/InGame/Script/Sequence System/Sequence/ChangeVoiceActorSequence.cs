using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public class ChangeVoiceActorSequence : ISequence
    {
        [OpenScriptButton(typeof(ChangeVoiceActorSequence))]
        [Description("ボイスを話すUIの立ち絵のみを変更するSequence")]
        [Header("変更する立ち絵")]
        [SerializeField] private AnnounceUiType _announceUi;
        
        private AnnounceUiController _announceUiController;
        
        public void SetData(SequenceData data)
        {
            _announceUiController = data.AnnounceUiController;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // 立ち絵を切り替える
            _announceUiController.ChangeAnnounceUi(_announceUi);
            
            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            
        }
    }
}
