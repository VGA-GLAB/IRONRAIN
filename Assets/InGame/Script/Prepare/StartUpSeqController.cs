using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public sealed class StartUpSeqController : MonoBehaviour
{
    [SerializeField] private Material[] _monitorMaterials = default;

    [SerializeField] private float _openEyesDuration = 1F;

    private readonly int _openEyesAmountPropertyId = Shader.PropertyToID("_OpenEyesAmount");

    private void Awake()
    {
        foreach (var n in _monitorMaterials)
        {
            n.SetFloat(_openEyesAmountPropertyId, 0F);
        }
    }

    public async UniTask StartUpSeqAsync(CancellationToken cancellationToken)
    {
        await MonitorOpenAsync(cancellationToken);
    }

    private async UniTask MonitorOpenAsync(CancellationToken cancellationToken)
    {
        await DOTween.To(() => 0F, value =>
        {
            foreach (var n in _monitorMaterials)
            {
                n.SetFloat(_openEyesAmountPropertyId, value);
            }
        }, 1, _openEyesDuration).ToUniTask(cancellationToken: cancellationToken);
    }
}
