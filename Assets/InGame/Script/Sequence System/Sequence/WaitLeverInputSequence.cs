using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitLeverInputSequence : AbstractWaitSequence
    {
        private enum LeverType
        {
            MoveInput,
            SortieInput
        }

        [OpenScriptButton(typeof(WaitLeverInputSequence))]
        [Description("レバーの入力を待つSequence。\nWaitLeverTypeをMoveInputにすると移動入力を、\nSortieInputにすると出撃入力を待つ")]
        [SerializeField, Header("どの状況のレバー入力を待つのか")]
        private LeverType _waitLeverType;

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);
            
            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            switch (_waitLeverType)
            {
                case LeverType.MoveInput:
                {
                    await UniTask.WaitUntil(() =>
                        InputProvider.Instance.LeftLeverDir != Vector3.zero ||
                        InputProvider.Instance.RightLeverDir != Vector3.zero,
                        cancellationToken: ct);
                    break;
                }
                case LeverType.SortieInput:
                {
                    await UniTask.WhenAll(
                        UniTask.WaitUntil(() => InputProvider.Instance.ThreeLeverDir.y > 0, cancellationToken: ct),
                        UniTask.WaitUntil(() => InputProvider.Instance.FourLeverDir.y > 0, cancellationToken: ct)
                    );
                    break;
                }
                default:
                {
                    await UniTask.CompletedTask;
                    break;
                }
            }
            
            loopCts.Cancel();
        }

        public override void Skip() { }
    }
}
