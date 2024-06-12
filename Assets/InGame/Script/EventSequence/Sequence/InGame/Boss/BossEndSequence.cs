using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class BossEndSequence : AbstractSequenceBase
{
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
    }
}
