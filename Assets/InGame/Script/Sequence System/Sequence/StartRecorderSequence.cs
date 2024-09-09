using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class StartRecorderSequence : ISequence
    {
        [OpenScriptButton(typeof(StartRecorderSequence))]
        [Description("動画を撮るSequenceです。ゲーム内で3回呼び出してください。\nこのシーケンスはすぐにつぎのシーケンスに進みます。")]
        [SerializeField, Header("何秒間動画を撮るのか")]
        private float _recordSeconds = 20F;

        private Recordings _recordings;
        
        public void SetData(SequenceData data)
        {
            _recordings = data.Recorder;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            Recording(ct).Forget();

            return UniTask.CompletedTask;
        }

        private async UniTaskVoid Recording(CancellationToken ct)
        {
            // 録画処理
            _recordings.StartRecord();

            await UniTask.WaitForSeconds(_recordSeconds, cancellationToken: ct);
            
            _recordings.StopRecord();
        }

        public void Skip() { }
    }
}
