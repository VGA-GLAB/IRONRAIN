using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class FallSequence : AbstractSequenceBase
{
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
    }
}
