using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class SoundSequence : ISequence
    {
        /// <summary>このシーケンス全体の時間</summary>
        [SerializeField] private float _totalSec = 0F;
        /// <summary>CueSheetName</summary>
        [SerializeField] private string _cueSheetName = "";
        /// <summary>CueName</summary>
        [SerializeField] private string _cueName = "";
        /// <summary>何秒後に流すか</summary>
        [SerializeField] private float _delaySec = 0F;

        public void SetParams(float totalSec, string cueSheetName, string cueName, float delaySec)
        {
            _totalSec = totalSec;
            _cueSheetName = cueSheetName;
            _cueName = cueName;
            _delaySec = delaySec;
        }

        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            PlaySoundAsync(ct).Forget();
            
            // 全体時間待って抜ける
            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        private async UniTask PlaySoundAsync(CancellationToken ct)
        {
            // 再生前の遅延
            await UniTask.WaitForSeconds(_delaySec, cancellationToken: ct);

            if (_cueSheetName == "BGM")
            {
                CriAudioManager.Instance.BGM.Play(_cueSheetName, _cueName);
            }
            else
            {
                CriAudioManager.Instance.SE.Play(_cueName, _cueName);
            }
        }

        public void Skip() { }
    }
}
