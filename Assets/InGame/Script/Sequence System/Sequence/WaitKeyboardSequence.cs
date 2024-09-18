using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


namespace IronRain.SequenceSystem
{
    public class WaitKeyboardSequence : AbstractWaitSequence
    {
        [OpenScriptButton(typeof(WaitKeyboardSequence))]
        [Description("キーボードの入力をまつSequence")]
        [Header("どのKeyの入力を待つのか"), SerializeField]
        private UnityEngine.InputSystem.Key _key;

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // Loop処理を待っている
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);

            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();

            await UniTask.WaitUntil(() => UnityEngine.InputSystem.Keyboard.current[_key].isPressed);

            loopCts.Cancel();
        }

        public override void Skip()
        {
        }
    }
}
