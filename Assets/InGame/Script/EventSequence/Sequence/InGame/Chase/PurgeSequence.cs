using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PurgeSequence : AbstractSequenceBase
{
    [SerializeField] PlayerController _playerController;
    
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        await _playerController.SeachState<PlayerStoryEvent>().StartJetPackPurge();
    }
}
