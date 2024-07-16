using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class WaitQTETutorialSequence : ISequence
    {
        [SerializeField] private float _qteStartDis = 400F;
        
        private SequenceData _data;

        public void SetParams(float qteStartDis)
        {
            _qteStartDis = qteStartDis;
        }
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            var enemies = new List<EnemyController>();

            if (_data.EnemyManager.TryGetEnemies(EnemyManager.Sequence.QTETutorial, enemies))
            {
                var id = enemies[0].BlackBoard.ID;

                await UniTask.WaitUntil(
                    () => (_data.PlayerController.transform.position - enemies[0].transform.position).sqrMagnitude <
                          _qteStartDis,
                    PlayerLoopTiming.Update,
                    ct);

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