using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class TutorialEnemyPauseSequence : ISequence
    {
        [SerializeField] private float _totalSec = 0F;
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
            _tutorialEnemy.Pause();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _tutorialEnemy.Pause();
        }
    }
}
