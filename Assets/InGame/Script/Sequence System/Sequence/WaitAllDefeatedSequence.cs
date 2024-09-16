using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitAllDefeatedSequence : AbstractWaitSequence
    {
        [OpenScriptButton(typeof(WaitAllDefeatedSequence))]
        [Description("指定したシーケンスの敵がすべて破壊されるまで、つぎのシーケンスに遷移するのを待つシーケンス")]
        [Header("対象のSequenceのID"), SerializeField] private int _targetSeq;

        private EnemyManager _enemyManager;

        private void SetParams(int targetSeq)
        {
            _targetSeq = targetSeq;
        }
        
        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _enemyManager = data.EnemyManager;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // Loop処理
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);

            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            await UniTask.WaitUntil(() => _enemyManager.IsAllDefeated(_targetSeq),
                cancellationToken: ct);
            
            loopCts.Cancel();
        }

        public override void Skip() { }
    }
}
