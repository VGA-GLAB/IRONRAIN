using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// どのようにキャラクターを制御するかを決める。
    /// </summary>
    public class BehaviorTree
    {
        public BehaviorTree(Transform transform, BossParams bossParams, BlackBoard blackBoard)
        {
            //
        }

        /// <summary>
        /// ビヘイビアツリーを実行。
        /// どのように制御するかをBlackBoardクラスに書き込む。
        /// </summary>
        public void Run(Choice choice)
        {
            //

            // ツリーを実行しても必ず行動として選択されるとは限らないので、
            // ノードの実行のたびに値を変化させるとバグる。
        }

        /// <summary>
        /// フレームを跨ぐ前に各ノードで書きこんだ内容を消す。
        /// </summary>
        public void ClearBlackBoardWritedValues()
        {
            //
        }
    }
}
