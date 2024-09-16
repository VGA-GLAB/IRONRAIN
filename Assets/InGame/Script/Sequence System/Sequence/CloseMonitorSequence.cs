using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public sealed class CloseMonitorSequence : ISequence
    {
        [OpenScriptButton(typeof(CloseMonitorSequence))]
        [Description("コックピットの外を映すモニターを閉じるシーケンス")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("モニターが閉じる時間(秒)"), SerializeField] private float _monitorCloseSec = 1F;

        private Material[] _materials;
        private static readonly int _openEyesAmount = Shader.PropertyToID("_OpenEyesAmount");

        private void SetParams(float totalSec, float monitorCloseSec)
        {
            _totalSec = totalSec;
            _monitorCloseSec = monitorCloseSec;
        }
        
        public void SetData(SequenceData data)
        {
            _materials = data.MonitorMaterials;
            
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            MonitorCloseAsync(ct).Forget(exceptionHandler);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }
        
        private async UniTask MonitorCloseAsync(CancellationToken ct)
        {
            await DOTween.To(() => 1F, value =>
            {
                foreach (var mat in _materials)
                {
                    mat.SetFloat(_openEyesAmount, value);
                }
            }, 0F, _monitorCloseSec).ToUniTask(cancellationToken: ct);
        }

        public void Skip()
        {
            foreach (var mat in _materials)
            {
                 mat.SetFloat(_openEyesAmount, 0F);
            }
        }
    }
}
