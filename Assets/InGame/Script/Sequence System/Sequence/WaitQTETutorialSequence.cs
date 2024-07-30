using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class WaitQTETutorialSequence : ISequence
    {
        private SequenceData _data;
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            var enemies = new List<EnemyController>();
            _data.PlayerController.PlayerEnvroment.AddState(PlayerStateType.NonTriggerQte);
            _data.PlayerController.PlayerEnvroment.AddState(PlayerStateType.NonAttack);

            if (_data.EnemyManager.TryGetEnemies(EnemyManager.Sequence.QTETutorial, enemies))
            {
                var id = enemies[0].BlackBoard.ID;

                await UniTask.WaitUntil(() => enemies[0].BlackBoard.IsApproachCompleted, PlayerLoopTiming.Update, ct);
                await UniTask.WaitForSeconds(1.0f, cancellationToken: ct);

                await WaitQTEAsync(id);
            }
        }

        private async UniTask WaitQTEAsync(System.Guid id)
        {
            // 比較用
            QTEResultType result = QTEResultType.Failure;

            var playerQTEModel = _data.PlayerController.SeachState<PlayerQTE>().QTEModel;

            // 成功するまでQTEを繰り返す
            while (result != QTEResultType.Success)
            {
                result = await playerQTEModel.StartQTE(id, QteType.NormalQte);
            }
        }

        public void Skip() { }
    }
}
