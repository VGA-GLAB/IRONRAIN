using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class PurgeSequence : AbstractSequenceBase
{
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
    }
}
