using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public sealed class InGameManager : MonoBehaviour
{
    [SerializeField] private ChaseSequenceController _chaseSequenceController = default;
    [SerializeField] private FirstAnnounceSeqController _firstAnnounceSeqController = default;
    [SerializeField] private AttackSeqController _attackSeqController = default;
    [SerializeField] private AvoidanceSeqController _avoidanceSeqController = default;
    [SerializeField] private BossStartSeqController _bossStartSeqController = default;
    [SerializeField] private PlayerController _playerController = default;
    [SerializeField] private GameObject _attackSeqEnemy = default;
    [SerializeField] private DebugScript _debugScript;
    [SerializeField, Tooltip("IProvidePlayerInformationInjectableを実装したComponentをアタッチしてください")]
    private Component[] _providePlayerInjectableComponents = default;

    public IProvidePlayerInformation PlayerInformation { get; } = new ProvidePlayerInformation();

    public interface IProvidePlayerInformation
    {
        public ISubject<Unit> StartQTE { get; }
        
        public ISubject<QTEResultType> EndQTE { get; }
        
        public float TimeScale { get; set; }
    }

    private class ProvidePlayerInformation : IProvidePlayerInformation
    {
        private Subject<Unit> _startQTE = new();
        public ISubject<Unit> StartQTE => _startQTE;

        private Subject<QTEResultType> _endQTE = new();
        public ISubject<QTEResultType> EndQTE => _endQTE;

        public float TimeScale { get; set; } = 1f;
    }

    private void Awake()
    {
        foreach (var n in _providePlayerInjectableComponents)
        {
            var temp = n as IProvidePlayerInformationInjectable;
            temp?.InjectProProvidePlayerInformation(PlayerInformation);
        }
        _debugScript.SetUp(_chaseSequenceController);
    }

    public async void InGameStart()
    {
        await ChaseManageAsync(this.GetCancellationTokenOnDestroy());
    }

    private void Update()
    {
        //Debug.Log(_chaseSequenceController.GetCurrentSequence());
    }

    private async UniTask ChaseManageAsync(CancellationToken cancellationToken)
    {
        await FirstAnnounceSequence(cancellationToken);

        await AvoidanceSequence(cancellationToken);

        await AttackSequence(cancellationToken);

        await TouchPanelSequence(cancellationToken);

        await LeverSequence(cancellationToken);

        await QTETutorialSequence(cancellationToken);

        await MultiBattleSequence(cancellationToken);

        await PurgeSequence(cancellationToken);

        await FallSequence(cancellationToken);

        await BossStartSequence(cancellationToken);

        await FirstFunnelSequence(cancellationToken);

        await ToggleButtonSequence(cancellationToken);

        await SecondFunnelSequence(cancellationToken);

        await BossAgainSequence(cancellationToken);

        await BreakLeftArmSequence(cancellationToken);

        await FirstBossQTESequence(cancellationToken);

        await SecondQTESequence(cancellationToken);

        await BossEndSequence(cancellationToken);
    }

    private async UniTask FirstAnnounceSequence(CancellationToken cancellationToken)
    {
        // FirstAttackSequence
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.FirstAnnounceSequence>();

        await _firstAnnounceSeqController.FirstAnnounceTaskSeqAsync(cancellationToken);
    }

    private async UniTask AvoidanceSequence(CancellationToken cancellationToken)
    {
        // AvoidanceSequence
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.AvoidanceSequence>();

        await _avoidanceSeqController.AvoidanceSeqAsync(cancellationToken);
    }

    private async UniTask AttackSequence(CancellationToken cancellationToken)
    {
        // AttackSequence
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.AttackSequence>();

        await _attackSeqController.AttackSeqAsync(cancellationToken);
    }

    private async UniTask TouchPanelSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.TouchPanelSequence>();

        // Todo:タッチパネルのチュートリアルの終了を待つ処理に差し替える
        //Debug.Log("タッチパネルのチュートリアル中");
        await UniTask.WaitForSeconds(1F, cancellationToken: cancellationToken);
        //Debug.Log("タッチパネルのチュートリアル終了");
    }

    private async UniTask LeverSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.LeverSequence>();

        // Todo:レバーのチュートリアルを待つ処理に書き直す
        //Debug.Log("Leverのチュートリアル中");
        await UniTask.WaitForSeconds(1F, cancellationToken: cancellationToken);
        //Debug.Log("Leverのチュートリアル終了");
    }

    private async UniTask QTETutorialSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.QTETutorialSequence>();

        // Todo:QTEのチュートリアルの終了を待つ処理に書き換える
        //Debug.Log("QTEのチュートリアル中");
        await UniTask.WaitForSeconds(1F, cancellationToken: cancellationToken);
        //Debug.Log("QTEのチュートリアル終了");
    }

    private async UniTask MultiBattleSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.MultiBattleSequence>();

        var storyEvent = _playerController.SeachState<PlayerStoryEvent>();
        //Debug.Log("道中戦の途中");
        await UniTask.WaitUntil(storyEvent.GoalCenterPoint, cancellationToken: cancellationToken);
        //Debug.Log("道中戦終了");
    }

    private async UniTask PurgeSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.PurgeSequence>();
        //Debug.Log("Purge中");
        await _playerController.SeachState<PlayerStoryEvent>().StartJetPackPurge();
       // Debug.Log("Purge終了");
    }

    private async UniTask FallSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.FallSequence>();
        await _playerController.SeachState<PlayerStoryEvent>().StartFall();
    }
    
    private async UniTask BossStartSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.BossStartSequence>();
        _bossStartSeqController.SetUp(_playerController);
        await _bossStartSeqController.BossStart();
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask FirstFunnelSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.FirstFunnelSequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask ToggleButtonSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.ToggleButtonSequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask SecondFunnelSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.SecondFunnelSequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask BossAgainSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.BossAgainSequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask BreakLeftArmSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.BreakLeftArmSequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask FirstBossQTESequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.FirstBossQTESequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask SecondQTESequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.SecondQTESequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
    
    private async UniTask BossEndSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.BossEndSequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
}
