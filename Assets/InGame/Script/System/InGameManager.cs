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
    [SerializeField] private GameObject _attackSeqEnemy = default;
    [SerializeField, Tooltip("IProvidePlayerInformationInjectableを実装したComponentをアタッチしてください")]
    private Component[] _providePlayerInjectableComponents = default;

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
        var providePlayerInformation = new ProvidePlayerInformation();
        
        foreach (var n in _providePlayerInjectableComponents)
        {
            var temp = n as IProvidePlayerInformationInjectable;
            temp?.InjectProProvidePlayerInformation(providePlayerInformation);
        }
    }

    private async void Start()
    {
        await ChaseManageAsync(this.GetCancellationTokenOnDestroy());
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
    }

    private async UniTask FirstAnnounceSequence(CancellationToken cancellationToken)
    {
        // FirstAttackSequence
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.FirstAnnounceSequence>();

        // Todo:アナウンスの処理が出来上がった場合修正する
        Debug.Log("オペレーターアナウンス中");
        await UniTask.WaitForSeconds(10F, cancellationToken: cancellationToken);
        Debug.Log("アナウンス終了");
    }

    private async UniTask AvoidanceSequence(CancellationToken cancellationToken)
    {
        // AvoidanceSequence
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.AvoidanceSequence>();

        // Todo:回避チュートリアルが出来上がった場合修正する
        Debug.Log("回避のチュートリアル中");
        await UniTask.WaitForSeconds(5F, cancellationToken: cancellationToken);
        Debug.Log("回避のチュートリアル終了");
    }

    private async UniTask AttackSequence(CancellationToken cancellationToken)
    {
        // AttackSequence
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.AttackSequence>();

        // 現在チュートリアルの敵が死んだら終了するようにしてある。
        Debug.Log("攻撃のチュートリアル中");
        await UniTask.WaitUntil(() => !_attackSeqEnemy, cancellationToken: cancellationToken);
        Debug.Log("攻撃のチュートリアル終了");
    }

    private async UniTask TouchPanelSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.TouchPanelSequence>();

        // Todo:タッチパネルのチュートリアルの終了を待つ処理に差し替える
        Debug.Log("タッチパネルのチュートリアル中");
        await UniTask.WaitForSeconds(5F, cancellationToken: cancellationToken);
        Debug.Log("タッチパネルのチュートリアル終了");
    }

    private async UniTask LeverSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.LeverSequence>();

        // Todo:レバーのチュートリアルを待つ処理に書き直す
        Debug.Log("Leverのチュートリアル中");
        await UniTask.WaitForSeconds(5F, cancellationToken: cancellationToken);
        Debug.Log("Leverのチュートリアル終了");
    }

    private async UniTask QTETutorialSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.QTETutorialSequence>();

        // Todo:QTEのチュートリアルの終了を待つ処理に書き換える
        Debug.Log("QTEのチュートリアル中");
        await UniTask.WaitForSeconds(5F, cancellationToken: cancellationToken);
        Debug.Log("QTEのチュートリアル終了");
    }

    private async UniTask MultiBattleSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.MultiBattleSequence>();

        Debug.Log("道中戦の途中");
        await UniTask.WaitForSeconds(10F, cancellationToken: cancellationToken);
        Debug.Log("道中戦終了");
    }

    private async UniTask PurgeSequence(CancellationToken cancellationToken)
    {
        _chaseSequenceController.ChangeSequence<ChaseSequenceController.PurgeSequence>();
        
        Debug.Log("Purge中");
        await UniTask.WaitForSeconds(5f, cancellationToken: cancellationToken);
        Debug.Log("Purge終了");
    }
}
