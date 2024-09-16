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
    public class TutorialQTESequence : ISequence
    {
        [OpenScriptButton(typeof(TutorialQTESequence)),
        Description("TutorialのQTEを待つためのSequence"),
        Header("QTEのタイプ"),
        SerializeField]
        private QTEState _qteType = QTEState.QtePreparation;
        
        private PlayerQTEModel _playerQteModel;
        
        public void SetData(SequenceData data)
        {
            _playerQteModel = data.PlayerController.SeachState<PlayerQTE>().QTEModel;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await _playerQteModel.TutorialQteCallseparately(_qteType, ct);
        }

        public void Skip() { }
    }
}
