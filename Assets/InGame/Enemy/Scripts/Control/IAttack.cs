using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 攻撃処理のインターフェース
    /// </summary>
    public interface IAttack
    {
        public void Attack();
    }
}
