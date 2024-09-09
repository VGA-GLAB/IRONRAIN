using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.SequenceSystem;
using UnityEngine;

namespace IronRain
{
    public class RecordCompleteSequence : ISequence
    {
        private FFmpegConcatenate _fFmpegConcatenate;

        public void SetData(SequenceData data)
        {
            _fFmpegConcatenate = data.Concatenate;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // 動画を繋げる処理
            _fFmpegConcatenate.ConcatenateVideos();

            return UniTask.CompletedTask;
        }

        public void Skip() { }
    }
}
