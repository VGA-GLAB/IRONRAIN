using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PrepareGameManager : MonoBehaviour
{
    [SerializeField] private PrepareSequenceController _prepareSequenceController = default;
    [SerializeField] private PrepareStartController _prepareStartController = default;

    private async void Start()
    {
        await ManagePrepareAsync(this.GetCancellationTokenOnDestroy());
    }

    private async UniTask ManagePrepareAsync(CancellationToken cancellationToken)
    {
        await _prepareStartController.PrepareStartAsync(cancellationToken);
        _prepareSequenceController.ChangeSequence<PrepareSequenceController.StartUpSequence>();

        await StartUpSeqAsync(cancellationToken);

        await PrepareSortieSeqAsync(cancellationToken);

        await SortieSeqAsync(cancellationToken);
    }

    private async UniTask StartUpSeqAsync(CancellationToken cancellationToken)
    {

    }

    private async UniTask PrepareSortieSeqAsync(CancellationToken cancellationToken)
    {

    }

    private async UniTask SortieSeqAsync(CancellationToken cancellationToken)
    {

    }
}
