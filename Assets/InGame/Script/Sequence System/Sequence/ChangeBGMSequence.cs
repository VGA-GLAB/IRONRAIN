using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class ChangeBGMSequence : ISequence
    {
        [OpenScriptButton(typeof(ChangeBGMSequence))]
        [Description("BGMを変更するためのSequence")]
        [SerializeField, Header("次のSequenceに遷移するまでの時間(秒)")]
        private float _totalSec = 0F;

        [SerializeField, Header("流すBGMの種類")]
        private BGM.BGMID _bgmid;
        
        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            PlayBGM();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        private void PlayBGM()
        {
            BGM.Instance.PlayBGM((int)_bgmid);
        }

        public void Skip()
        {
            PlayBGM();
        }
    }
}
