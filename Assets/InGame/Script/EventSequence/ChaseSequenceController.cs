using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseSequenceController : SequenceControllerBase
{
    [SerializeField] private FirstAnnounceSequence _firstAnnounceSequence = default;
    [SerializeField] private AvoidanceSequence _avoidanceSequence = default;
    [SerializeField] private AttackSequence _attackSequence = default;
    [SerializeField] private TouchPanelSequence _touchPanelSequence = default;
    [SerializeField] private LeverSequence _leverSequence = default;
    [SerializeField] private QTETutorialSequence _qteTutorialSequence = default;
    [SerializeField] private MultiBattleSequence _multiBattleSequence = default;
    [SerializeField] private PurgeSequence _purgeSequence = default;

    private void Awake()
    {
        _sequences = new List<EventSequenceBase>()
        {
            _firstAnnounceSequence, _avoidanceSequence, _attackSequence, _touchPanelSequence, _leverSequence,
            _qteTutorialSequence, _multiBattleSequence, _purgeSequence
        };
    }
    
    [Serializable] public sealed class FirstAnnounceSequence : EventSequenceBase { }
    [Serializable] public sealed class AvoidanceSequence : EventSequenceBase { }
    [Serializable] public sealed class AttackSequence : EventSequenceBase { }
    [Serializable] public sealed class TouchPanelSequence : EventSequenceBase { }
    [Serializable] public sealed class LeverSequence : EventSequenceBase { }
    [Serializable] public sealed class QTETutorialSequence : EventSequenceBase { }
    [Serializable] public sealed class MultiBattleSequence : EventSequenceBase { }
    [Serializable] public sealed class PurgeSequence : EventSequenceBase { }
}
