using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class SoundSequence : ISequence
    {
        [OpenScriptButton(typeof(SoundSequence))]
        [Description("CueSheetとCueNameを指定して、音を流すシーケンス")]
        /// <summary>このシーケンス全体の時間</summary>
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        /// <summary>CueSheetName</summary>
        [Header("流すCueのCueSheetName"), SerializeField] private string _cueSheetName = "";
        /// <summary>CueName</summary>
        [Header("流すCue"), SerializeField] private string _cueName = "";
        /// <summary>何秒後に流すか</summary>
        [Header("何秒後に流すのか"), SerializeField] private float _delaySec = 0F;

        [Header("Stopをかける際は、0以上のIDを指定してください"), SerializeField]
        private int _id = -1;
        
        public void SetParams(float totalSec, string cueSheetName, string cueName, float delaySec)
        {
            _totalSec = totalSec;
            _cueSheetName = cueSheetName;
            _cueName = cueName;
            _delaySec = delaySec;
        }

        private SequenceData.SoundSequenceManager _soundSequenceManager;
        private Transform _voiceTransform;
        
        public void SetData(SequenceData data)
        {
            _soundSequenceManager = data.SoundManager;
            _voiceTransform = data.VoiceTransform;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            PlaySoundAsync(ct).Forget(exceptionHandler);
            
            // 全体時間待って抜ける
            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        private async UniTask PlaySoundAsync(CancellationToken ct)
        {
            // 再生前の遅延
            await UniTask.WaitForSeconds(_delaySec, cancellationToken: ct);

            var index = 0;
            
            if (_cueSheetName == "BGM")
            {
                index = CriAudioManager.Instance.BGM.Play(_cueSheetName, _cueName);
            }
            else
            {
                index = CriAudioManager.Instance.CockpitSE.Play3D(_voiceTransform.position, _cueSheetName, _cueName);
            }
            
            if (_id > -1) _soundSequenceManager.RegisterIndex(_id, index);
        }

        public void Skip()
        {
            var index = 0;
            
            if (_cueSheetName == "BGM")
            {
                index = CriAudioManager.Instance.BGM.Play(_cueSheetName, _cueName);
            }
            else
            {
                index = CriAudioManager.Instance.CockpitSE.Play3D(_voiceTransform.position, _cueSheetName, _cueName);
            }
            
            if (_id > -1) _soundSequenceManager.RegisterIndex(_id, index);
        }
    }
}
