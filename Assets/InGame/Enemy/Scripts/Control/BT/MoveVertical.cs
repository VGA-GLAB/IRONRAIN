using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// 逃げる際に垂直移動する。
    /// </summary>
    public class MoveVertical : Node
    {
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        public MoveVertical(EnemyParams enemyParams, BlackBoard blackBoard)
        {
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
            // 上に逃げる。
            _blackBoard.AddMovementOption(Choice.Escape, Vector3.up, _params.Move.EscapeSpeed);

            return State.Success;
        }
    }
}
