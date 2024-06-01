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
    [SerializeField] private FallSequence _fallSequence = default;
    [SerializeField] private BossStartSequence _bossStartSequence = default;
    [SerializeField] private FirstFunnelSequence _firstFunnelSequence = default;
    [SerializeField] private ToggleButtonSequence _toggleButtonSequence = default;
    [SerializeField] private SecondFunnelSequence _secondFunnelSequence = default;
    [SerializeField] private BossAgainSequence _bossAgainSequence = default;
    [SerializeField] private BreakLeftArmSequence _breakLeftArmSequence = default;
    [SerializeField] private FirstBossQTESequence _firstBossQteSequence = default;
    [SerializeField] private SecondQTESequence _secondQteSequence = default;
    [SerializeField] private BossEndSequence _bossEndSequence = default;
    
    private void Awake()
    {
        _sequences = new List<EventSequenceBase>()
        {
            _firstAnnounceSequence, _avoidanceSequence, _attackSequence, _touchPanelSequence, _leverSequence,
            _qteTutorialSequence, _multiBattleSequence, _purgeSequence, _fallSequence, _bossStartSequence, 
            _firstFunnelSequence, _toggleButtonSequence, _secondFunnelSequence, _bossAgainSequence, 
            _breakLeftArmSequence, _firstBossQteSequence, _secondQteSequence, _bossEndSequence
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
    [Serializable] public sealed class FallSequence : EventSequenceBase { }
    [Serializable] public sealed class BossStartSequence : EventSequenceBase { }
    [Serializable] public sealed class FirstFunnelSequence : EventSequenceBase { }
    [Serializable] public sealed class ToggleButtonSequence : EventSequenceBase { }
    [Serializable] public sealed class SecondFunnelSequence : EventSequenceBase { }
    [Serializable] public sealed class BossAgainSequence : EventSequenceBase { }
    [Serializable] public sealed class BreakLeftArmSequence : EventSequenceBase { }
    [Serializable] public sealed class FirstBossQTESequence : EventSequenceBase { }
    [Serializable] public sealed class SecondQTESequence : EventSequenceBase { }
    [Serializable] public sealed class BossEndSequence : EventSequenceBase { }
}
