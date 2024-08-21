using System;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public sealed class WaitLongPressSequence : ISequence
    {
        [OpenScriptButton(typeof(WaitLongPressSequence))]
        [Description("長押し入力をまつSequence")]
        [SerializeField, Header("長押しをする入力")]
        private InputProvider.InputType _inputType;
        [SerializeField, Header("何秒長押しするのか")]
        private float _pressSec;
        
        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await WaitLongPressAsync(ct, exceptionHandler);
        }

        private async UniTask WaitLongPressAsync(CancellationToken ct, Action<Exception> exceptionHandler)
        {
            bool completed = false;
            
            while (true)
            {
                await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(_inputType), cancellationToken: ct);

                Debug.Log("長押し開始");

                var elapsed = 0F;
                while (true)
                {
                    await UniTask.Yield(ct);

                    elapsed += Time.deltaTime;

                    if (_pressSec < elapsed)
                    {
                        completed = true;
                    }

                    if (completed || !InputProvider.Instance.GetStayInput(_inputType)) break;
                }

                if (completed) break;
            }
        }

        public void Skip() { }
    }
}
