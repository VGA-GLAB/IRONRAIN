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
        [OpenScriptButton(typeof(OpenMonitorSequence))]
        [Description("コックピットの外を映すモニターを起動するシーケンス")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("モニターが開く時間(秒)"), SerializeField] private float _monitorOpenSec = 1F;

        private Material[] _materials;
        private static readonly int _openEyesAmount = Shader.PropertyToID("_OpenEyesAmount");

        private void SetParams(float totalSec, float monitorOpenSec)
        {
            _totalSec = totalSec;
            _monitorOpenSec = monitorOpenSec;
        }
        
        private void Init()
        {
            foreach (var mat in _materials)
            {
                mat.SetFloat(_openEyesAmount, 0F);
            }
        }
        
        public void SetData(SequenceData data)
        {
            _materials = data.MonitorMaterials;
            
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
                foreach (var mat in _materials)
                {
                    mat.SetFloat(_openEyesAmount, value);
                }
            }, 1F, _monitorOpenSec).ToUniTask(cancellationToken: ct);
        }

        public void Skip()
        {
            foreach (var mat in _materials)
            {
                 mat.SetFloat(_openEyesAmount, 1F);
            }
        }
    }
}
