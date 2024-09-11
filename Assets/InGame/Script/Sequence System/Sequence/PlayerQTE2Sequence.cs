using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    /// <summary>新しく作り替えたもの,FirstとQTEを一つにまとめました。</summary>
    public sealed class PlayerQTE2Sequence : ISequence
    {
        [OpenScriptButton(typeof(PlayerQTE2Sequence))]
        [Description("PlayerのBossQTE1の入力を待つSequence")]
        [SerializeField, Header("PlayerのQTEのタイプ")]
        private QTEState _qteType;

        private PlayerController _playerController;

        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await PlayQTEAsync(ct, exceptionHandler);
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
            await qteModel.BossQte2Callseparately(_qteType, qteCts.Token);
        }

        public void Skip()
        {
        }
    }
}