using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public sealed class WaitToggleSequence : AbstractWaitSequence
    {
        [OpenScriptButton(typeof(WaitToggleSequence))]
        [Description("指定した二つの入力があるまで、つぎのシーケンスへの遷移を待つシーケンス")]
        [Header("どの入力を待つのか"), SerializeField] private InputProvider.InputType _toggleButton1 = InputProvider.InputType.Toggle1;
        [SerializeField] private InputProvider.InputType _toggleButton2 = InputProvider.InputType.Toggle2;

        private Transform _playerTransform;
        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _playerTransform = data.PlayerTransform;

        }

        public void SetParams(InputProvider.InputType toggleButton1, InputProvider.InputType toggleButton2)
        {
            _toggleButton1 = toggleButton1;
            _toggleButton2 = toggleButton2;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // ループ中のCancellationToken
            using var waitingCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, waitingCts.Token);
            
            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();

            // 二つの入力を待つ
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(_toggleButton1), cancellationToken: ct);
            CriAudioManager.Instance.SE.Play3D(_playerTransform.position, "SE", "SE_Purge");
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(_toggleButton2), cancellationToken: ct);
            CriAudioManager.Instance.SE.Play3D(_playerTransform.position, "SE", "SE_Purge");

            waitingCts.Cancel();
        }

        public override void Skip()
        {
        }
    }
}
