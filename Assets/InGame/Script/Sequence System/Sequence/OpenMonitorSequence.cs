using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public sealed class OpenMonitorSequence : ISequence
    {
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private float _monitorOpenSec = 1F;

        private SequenceData _data;
        private readonly int _openEyesAmountPropertyId = UnityEngine.Shader.PropertyToID("_OpenEyesAmount");

        private void SetParams(float totalSec, float monitorOpenSec)
        {
            _totalSec = totalSec;
            _monitorOpenSec = monitorOpenSec;
        }
        
        private void Init()
        {
            foreach (var mat in _data.MonitorMaterials)
            {
                mat.SetFloat(_openEyesAmountPropertyId, 0F);
            }
        }
        
        public void SetData(SequenceData data)
        {
            _data = data;
            
            Init();
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            MonitorOpenAsync(ct).Forget(exceptionHandler);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }
        
        private async UniTask MonitorOpenAsync(CancellationToken ct)
        {
            await DOTween.To(() => 0, value =>
            {
                foreach (var mat in _data.MonitorMaterials)
                {
                    mat.SetFloat(_openEyesAmountPropertyId, value);
                }
            }, 1F, _monitorOpenSec).ToUniTask(cancellationToken: ct);
        }

        public void Skip()
        {
            foreach (var mat in _data.MonitorMaterials)
            {
                 mat.SetFloat(_openEyesAmountPropertyId, 1F);
            }
        }
    }
}
