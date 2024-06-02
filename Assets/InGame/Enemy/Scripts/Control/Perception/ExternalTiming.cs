using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 外部から任意のタイミングで処理を呼んだ場合にタイミングを合わせるよう調整する。
    /// 現在はチュートリアル用。
    /// </summary>
    public class ExternalTiming
    {
        private BlackBoard _blackBoard;

        private bool _attackTrigger;
        private bool _isPause;

        public ExternalTiming(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        /// <summary>
        /// Updateのタイミングで黒板に反映する。
        /// </summary>
        public void Update()
        {
            _blackBoard.ExternalAttackTrigger = _attackTrigger;
            _attackTrigger = false;

            _blackBoard.IsExternalPause = _isPause;
        }

        /// <summary>
        /// 攻撃タイミングをトリガーする。
        /// </summary>
        public void AttackTrigger()
        {
            _attackTrigger = true;
        }

        /// <summary>
        /// ポーズする。
        /// </summary>
        public void Pause()
        {
            _isPause = true;
        }

        /// <summary>
        /// ポーズ解除する。
        /// </summary>
        public void Resume()
        {
            _isPause = false;
        }
    }
}
