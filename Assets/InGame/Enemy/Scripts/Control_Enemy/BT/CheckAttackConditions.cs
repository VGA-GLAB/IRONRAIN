using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// 視界内にプレイヤーがおり、攻撃タイミングかどうかをチェック。
    /// </summary>
    public class CheckAttackConditions : Node
    {
        private Transform _transform;
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        public CheckAttackConditions(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _transform = transform;
            _params = enemyParams;
            _blackBoard = blackBoard;
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
            if (CheckFOV())
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
