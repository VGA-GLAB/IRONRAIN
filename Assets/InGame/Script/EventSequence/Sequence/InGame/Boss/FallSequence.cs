using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class FallSequence : AbstractSequenceBase
{
    [SerializeField] private PlayerController _playerController;

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        await _playerController.SeachState<PlayerStoryEvent>().StartFall();
    }
}
