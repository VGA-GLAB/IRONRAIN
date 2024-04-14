using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    public static class StateExtensions
    {
        /// <summary>
        /// 死亡もしくは撤退をチェックする。
        /// </summary>
        public static bool IsExit(BlackBoard b)
        {
            foreach (ActionPlan plan in b.ActionOptions)
            {
                if (plan.Choice == Choice.Broken || plan.Choice == Choice.Escape)
                {
                    return true;
                }
            }

            return false;
        }
    }
}