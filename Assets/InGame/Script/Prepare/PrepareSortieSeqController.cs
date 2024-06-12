using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PrepareSortieSeqController : MonoBehaviour
{
    [SerializeField] private HatchController _firstHatch = default;

    [SerializeField] private HatchController.OpenDoorData _firstHatchData = default;

    [SerializeField] private Transform _cameraTransform = default;

    [SerializeField] private Transform _moveTarget = default;

    [SerializeField] private float _moveTargetDuration = 10F;
    
    public async UniTask PrepareSortieSeqAsync(CancellationToken cancellationToken)
    {
        await _firstHatch.OpenDoorAsync(_firstHatchData, cancellationToken);

        await _cameraTransform.DOMove(_moveTarget.position, _moveTargetDuration)
            .ToUniTask(cancellationToken: cancellationToken);
    }
}
