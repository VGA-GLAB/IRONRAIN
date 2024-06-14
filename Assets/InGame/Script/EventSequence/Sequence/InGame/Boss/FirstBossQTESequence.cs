using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy.Control;

public sealed class FirstBossQTESequence : AbstractSequenceBase
{
    [SerializeField] private EnemyManager _enemyManager;

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        _enemyManager.BossFirstQte();
    }
}
