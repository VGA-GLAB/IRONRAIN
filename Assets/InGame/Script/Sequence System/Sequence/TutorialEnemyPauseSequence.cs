using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class TutorialEnemyPauseSequence : ISequence
    {
        [OpenScriptButton(typeof(TutorialEnemyPauseSequence))]
        [Description("攻撃を回避するチュートリアルで、敵を一時停止させるシーケンス")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        private EnemyController _tutorialEnemy;

        public void SetParams(float totalSec)
        {
            _totalSec = totalSec;
        }
        
        public void SetData(SequenceData data)
        {
            _tutorialEnemy = data.TutorialEnemy;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
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
