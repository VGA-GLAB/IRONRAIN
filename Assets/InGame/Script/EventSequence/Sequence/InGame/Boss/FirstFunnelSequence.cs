using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy.Control;

public sealed class FirstFunnelSequence : AbstractSequenceBase
{
    [SerializeField] private EnemyManager _enemyManager;
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        // 40秒後、ファンネル展開。
        await UniTask.WaitForSeconds(40.0f, cancellationToken: ct);
        _enemyManager.FunnelExpand();
    }
}
