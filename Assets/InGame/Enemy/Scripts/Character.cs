using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    // 雑魚、ボス、NPC、ファンネルで共通した基底クラス。
    // オブジェクトが以下のような構成になっている前提。
    // root
    //  DynamicOffset
    //   StaticOffset
    //    Rotate
    //     3Dモデル
    public class Character : MonoBehaviour
    {
        protected Transform FindDynamicOffset() => transform.FindChildRecursive("DynamicOffset");
        protected Transform FindStaticOffset() => transform.FindChildRecursive("StaticOffset");
        protected Transform FindRotate() => transform.FindChildRecursive("Rotate");
    }
}
