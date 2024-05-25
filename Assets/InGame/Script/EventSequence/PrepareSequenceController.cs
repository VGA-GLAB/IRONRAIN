using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PrepareSequenceController : SequenceControllerBase
{
    [SerializeField] private PrepareStartSequence _prepareStartSequence = default;
    [SerializeField] private StartUpSequence _startUpSequence = default;
    [SerializeField] private PrepareSortieSequence _prepareSortieSequence = default;
    [SerializeField] private SortieSequence _sortieSequence = default;

    private void Awake()
    {
        _sequences = new()
        {
            _prepareStartSequence, _startUpSequence, _prepareSortieSequence, _sortieSequence
        };
    }

    [Serializable] public sealed class PrepareStartSequence : EventSequenceBase { }
    [Serializable] public sealed class StartUpSequence : EventSequenceBase { }
    [Serializable] public sealed class PrepareSortieSequence : EventSequenceBase { }
    [Serializable] public sealed class SortieSequence : EventSequenceBase { }
}
