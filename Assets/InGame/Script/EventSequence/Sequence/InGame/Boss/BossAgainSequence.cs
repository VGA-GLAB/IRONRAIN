using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class BossAgainSequence : AbstractSequenceBase
{
    [SerializeField] private float _battleSec = 20F;
    
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        await UniTask.WaitForSeconds(_battleSec, cancellationToken: ct);
    }
}
