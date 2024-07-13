using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class TutorialEnemyAttackSequence : ISequence
    {
        [SerializeField] private float _totalSec;
        private EnemyController _tutorialEnemy;

        public void SetParams(float totalSec)
        {
            _totalSec = totalSec;
        }
        
        public void SetData(SequenceData data)
        {
            _tutorialEnemy = data.TutorialEnemy;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _tutorialEnemy.Attack();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _tutorialEnemy.Attack();
        }
    }
}
