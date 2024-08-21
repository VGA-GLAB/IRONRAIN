using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitLeverInputSequence : ISequence
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
        
        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
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
                    await UniTask.WaitUntil(() =>
                        InputProvider.Instance.ThreeLeverDir.y > 0 ||
                        InputProvider.Instance.FourLeverDir.y > 0,
                        cancellationToken: ct);
                    break;
                }
                default:
                {
                    await UniTask.CompletedTask;
                    break;
                }
            }
        }

        public void Skip() { }
    }
}
