using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitTutorialEnemyApproachSequence : ISequence
    {
        private EnemyController _tutorialEnemy;
        
        public void SetData(SequenceData data)
        {
            _tutorialEnemy = data.TutorialEnemy;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await UniTask.WaitUntil(() => _tutorialEnemy.BlackBoard.IsApproachCompleted, cancellationToken: ct);
        }

        public void Skip() { }
    }
}
