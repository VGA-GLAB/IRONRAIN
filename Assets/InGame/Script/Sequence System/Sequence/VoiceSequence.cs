using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class VoiceSequence : ISequence
    {
        [OpenScriptButton(typeof(VoiceSequence))]
        [Description("ボイスを再生させるシーケンスです。\nUIのボイスの話している人の立ち絵も切り替えます。")]
        [Header("このシーケンスを抜けるまでの時間")]
        [SerializeField] private float _totalSec = 0.5F;
        [Header("そのボイスを喋っている人")]
        [SerializeField] private AnnounceUiType _announceUiType;
        [Header("流すボイスのCueSheetName")] 
        [SerializeField] private string _cueSheetName;
        [Header("流すボイスのCueName")]
        [SerializeField] private string _cueName;
        [Header("声を流すまでの遅延")]
        [SerializeField] private float _delaySec = 0.0F;
        
        private AnnounceUiController _announceUiController;
        
        public void SetData(SequenceData data)
        {
            _announceUiController = data.AnnounceUiController;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            PlayVoice(ct).Forget(exceptionHandler);
            // Voiceの立ち絵を切り替える
            _announceUiController.ChangeAnnounceUi(_announceUiType);
            
            // シーケンスの全体の時間を待つ
            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        /// <summary>音を流す処理</summary>
        /// <param name="ct">CancellationToken</param>
        private async UniTask PlayVoice(CancellationToken ct)
        {
            await UniTask.WaitForSeconds(_delaySec, cancellationToken: ct);

            CriAudioManager.Instance.Voice.Play(_cueSheetName, _cueName);
        }

        public void Skip()
        {
        }
    }
}
