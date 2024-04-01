using Enemy.Control.BT;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 視界内にプレイヤーがおり、攻撃タイミングかどうかをチェック。
    /// </summary>
    public class CheckAttackConditions : Node
    {
        private Transform _transform;
        private Transform _rotate;
        private BlackBoard _blackBoard;
        private EnemyParams _params;

        public CheckAttackConditions(Transform transform, Transform rotate, BlackBoard blackBoard, EnemyParams enemyParams)
        {
            _transform = transform;
            _rotate = rotate;
            _blackBoard = blackBoard;
            _params = enemyParams;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // 視界は自身を中心とした球形なので、横や後ろにいる場合も攻撃してしまう。
            if (CheckFOV() && _blackBoard.NextAttackTime < Time.time)
            {
                return State.Success;
            }
            else return State.Failure;
        }

        // 視界に捉えたオブジェクトの中からプレイヤーを探す
        private bool CheckFOV()
        {
            foreach (Collider c in _blackBoard.FovEnter)
            {
                if (c.CompareTag(Const.PlayerTag)) return true;
            }

            foreach (Collider c in _blackBoard.FovStay)
            {
                if (c.CompareTag(Const.PlayerTag)) return true;
            }

            return false;
        }
    }
}
