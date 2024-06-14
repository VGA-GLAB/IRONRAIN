using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy.Control;

public sealed class MultiBattleSequence : AbstractSequenceBase
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyManager _enemyManager;

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        var storyEvent = _playerController.SeachState<PlayerStoryEvent>();
        //Debug.Log("道中戦の途中");
        await UniTask.WaitUntil(storyEvent.GoalCenterPoint, cancellationToken: ct);

        // 4機の味方機が後ろから突進する。
        _enemyManager.PlayNpcEvent(EnemyManager.Sequence.MultiBattle);
    }
}
