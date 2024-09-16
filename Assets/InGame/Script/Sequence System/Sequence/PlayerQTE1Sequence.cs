using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    /// <summary>新しく作り替えたもの,FirstとQTEを一つにまとめました。</summary>
    public sealed class PlayerQTE1Sequence : AbstractWaitSequence
    {
        [OpenScriptButton(typeof(PlayerQTE1Sequence))]
        [Description("PlayerのBossQTE1の入力を待つSequence")]
        [SerializeField, Header("PlayerのQTEのタイプ")]
        private QTEState _qteType;

        private PlayerController _playerController;

        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _playerController = data.PlayerController;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // Wait時のLoop処理
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(loopCts.Token, ct);
            
            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            await PlayQTEAsync(ct, exceptionHandler);
            
            loopCts.Cancel();
        }

        /// <summary>PlayerのQTEをよびだす</summary>
        private async UniTask PlayQTEAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            var qteModel = _playerController.SeachState<PlayerQTE>().QTEModel;

            // PlayerQTEを呼び出している
            var qteCts = new CancellationTokenSource();
            
            // CancellationTokenSourceをSequence全体のCTSにひもづける
            ct.Register(() =>
            {
                qteCts?.Cancel();
                qteCts?.Dispose();
            }).AddTo(qteCts.Token);
            
            qteModel.QTEFailureJudgment(qteCts, ct).Forget(exceptionHandler);
            await qteModel.BossQte1Callseparately(_qteType, qteCts.Token);
        }

        public override void Skip()
        {
        }
    }
}