using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class AbstractSequenceBase : MonoBehaviour
{
    public abstract UniTask PlaySequenceAsync(CancellationToken ct);

    public virtual void OnSkip() { }
}
