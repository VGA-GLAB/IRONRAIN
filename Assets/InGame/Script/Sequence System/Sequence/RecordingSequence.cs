using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Recording;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace IronRain.SequenceSystem
{
    public class RecordingSequence : ISequence
    {
        [OpenScriptButton(typeof(RecordingSequence))]
        [Description("このシーケンスに入ったら、指定された秒数動画を撮影します。\nこのシーケンスが呼ばれたらすぐにつぎのシーケンスに進みます。")]
        [SerializeField, Header("何秒間録画するのか")]
        private float _recordSec = 1F;
        
#if UNITY_EDITOR
        private CustomRecorderController _recorder;
#endif
        public void SetData(SequenceData data)
        {
#if UNITY_EDITOR
            _recorder = data.Recorder;
#endif
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
#if UNITY_EDITOR
            RecordAsync(ct).Forget(exceptionHandler);
#endif
            return UniTask.CompletedTask;
        }
        
#if UNITY_EDITOR
        private async UniTask RecordAsync(CancellationToken ct)
        {
            _recorder.StartRecording();

            await UniTask.WaitForSeconds(_recordSec, cancellationToken: ct);
            
            _recorder.StopRecording();

            await UniTask.CompletedTask;
        }
#endif

        public void Skip()
        {
        }
    }
}
