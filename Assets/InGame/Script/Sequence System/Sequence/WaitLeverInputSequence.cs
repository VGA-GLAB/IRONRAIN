using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitLeverInputSequence : ISequence
    {
        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await UniTask.WaitUntil(() =>
                InputProvider.Instance.LeftLeverDir != Vector3.zero ||
                InputProvider.Instance.RightLeverDir != Vector3.zero ||
                UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed, cancellationToken: ct);
        }

        public void Skip() { }
    }
}
