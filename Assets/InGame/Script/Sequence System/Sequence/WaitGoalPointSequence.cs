using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class WaitGoalPointSequence : ISequence
    {
        private PlayerController _playerController;
        
        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            var storyEvent = _playerController.SeachState<PlayerStoryEvent>();

            await UniTask.WaitUntil(storyEvent.GoalCenterPoint, cancellationToken: ct);
        }

        public void Skip() { }
    }
}
