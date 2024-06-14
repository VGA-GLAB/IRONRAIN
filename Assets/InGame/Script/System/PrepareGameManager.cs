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
    [SerializeField] private StartUpSeqController _startUpSeqController = default;
    [SerializeField] private PrepareSortieSeqController _prepareSortieSeqController = default;
    [SerializeField] private SortieController _sortieController = default;
    
    private void Start()
    {
        ManagePrepareAsync(this.GetCancellationTokenOnDestroy()).Forget();
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
        _prepareSequenceController.ChangeSequence<PrepareSequenceController.StartUpSequence>();
        await _prepareStartController.PrepareStartAsync(cancellationToken);
    }

    private async UniTask StartUpSeqAsync(CancellationToken cancellationToken)
    {
        _prepareSequenceController.ChangeSequence<PrepareSequenceController.StartUpSequence>();
        await _startUpSeqController.StartUpSeqAsync(cancellationToken);
    }

    private async UniTask PrepareSortieSeqAsync(CancellationToken cancellationToken)
    {
        _prepareSequenceController.ChangeSequence<PrepareSequenceController.PrepareSortieSequence>();
        await _prepareSortieSeqController.PrepareSortieSeqAsync(cancellationToken);
    }

    private async UniTask SortieSeqAsync(CancellationToken cancellationToken)
    {
        _prepareSequenceController.ChangeSequence<PrepareSequenceController.SortieSequence>();
        await _sortieController.SortieSeqAsync(cancellationToken);
        _prepareSequenceController.EndSequence();
    }
}
