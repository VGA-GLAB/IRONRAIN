using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    /// <summary>新しく作り替えたもの,FirstとQTEを一つにまとめました。</summary>
    public sealed class PlayerQTE1Sequence : ISequence
    {
        [OpenScriptButton(typeof(PlayerQTE1Sequence))]
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
            using (var qteCt = new CancellationTokenSource())
            {
                qteModel.QTEFailureJudgment(qteCt, ct).Forget(exceptionHandler);
                await qteModel.BossQte1Callseparately(_qteType, qteCt.Token);
            }
        }

        public void Skip()
        {
        }
    }
}