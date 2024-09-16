using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using IronRain.SequenceSystem;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class TutorialQTESequence : AbstractWaitSequence
    {
        [OpenScriptButton(typeof(TutorialQTESequence)),
        Description("TutorialのQTEを待つためのSequence"),
        Header("QTEのタイプ"),
        SerializeField]
        private QTEState _qteType = QTEState.QtePreparation;
        
        private PlayerQTEModel _playerQteModel;
        
        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _playerQteModel = data.PlayerController.SeachState<PlayerQTE>().QTEModel;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // ループ処理のためのCancellationTokenを用意する
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);

            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            await _playerQteModel.TutorialQteCallseparately(_qteType, ct);

            loopCts.Cancel();
        }

        public override void Skip() { }
    }
}
