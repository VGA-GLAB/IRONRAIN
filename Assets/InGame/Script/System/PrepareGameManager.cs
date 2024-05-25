using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PrepareGameManager : MonoBehaviour
{
    [SerializeField] private PrepareSequenceController _prepareSequenceController = default;

    private async void Start()
    {
        await ManagePrepareAsync(this.GetCancellationTokenOnDestroy());
    }

    private async UniTask ManagePrepareAsync(CancellationToken cancellationToken)
    {
        await PrepareStartSeqAsync(cancellationToken);

        await StartUpSeqAsync(cancellationToken);

        await PrepareSortieSeqAsync(cancellationToken);

        await SortieSeqAsync(cancellationToken);
    }

    private async UniTask PrepareStartSeqAsync(CancellationToken cancellationToken)
    {

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
