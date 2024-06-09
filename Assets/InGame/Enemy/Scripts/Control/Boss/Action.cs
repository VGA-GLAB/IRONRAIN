using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class Action
    {
        private BlackBoard _blackBoard;
        private Transform _transform;
        private IReadOnlyCollection<FunnelController> _funnels;

        public Action(BlackBoard blackBoard, Transform transform, IReadOnlyCollection<FunnelController> funnels)
        {
            _blackBoard = blackBoard;
            _transform = transform;
            _funnels = funnels;
        }

        public void OnStartEvent()
        {
            foreach (FunnelController f in _funnels)
            {
                f.OpenAnimation();
            }
        }

        public void UpdateEvent()
        {
            // 座標を直接書き換える。
            // deltaTimeぶんの移動を上書きする恐れがあるので移動より先。
            while (_blackBoard.WarpOptions.TryDequeue(out WarpPlan plan))
            {
                if (plan.Choice != Choice.Idle) continue;

                _transform.position = plan.Position;
            }
        }
    }
}
