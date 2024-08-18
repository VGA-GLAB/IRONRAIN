using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    // 雑魚、ボス、NPC、ファンネルで共通した基底クラス。
    // オブジェクトが以下のような構成になっている前提。
    // root
    //  Offset
    //   Rotate
    //    3Dモデル
    //   NoRotate
    public class Character : MonoBehaviour
    {
        public Transform FindOffset() => transform.FindChildRecursive("Offset");
        public Transform FindRotate() => transform.FindChildRecursive("Rotate");
        protected Transform FindPlayer() => GameObject.FindGameObjectWithTag(Const.PlayerTag).transform;
    }
}
