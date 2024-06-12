﻿using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Control
{
    /// <summary>
    /// アニメーションイベントにフックする用のコールバックをまとめてある。
    /// このスクリプトはAnimatorと同じオブジェクトにアタッチすること。
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationEvent : MonoBehaviour
    {
        /// <summary>
        /// 近接攻撃の判定を出すタイミング。
        /// 遠距離攻撃の場合はこのタイミングで弾を発射する。
        /// </summary>
        public event UnityAction OnFireStart;
        /// <summary>
        /// 近接攻撃の判定を消すタイミング。
        /// </summary>
        public event UnityAction OnFireEnd;

        // アニメーションイベントに登録するメソッド群。
        // アニメーションの終了と同じフレームに登録すると意図した挙動にならない可能性がある。
        // Unityのイベント関数の順番の問題？
        // 対策として終了1フレーム手前でイベントを呼ぶなど工夫が必要。
        public void FireStart() => OnFireStart?.Invoke();
        public void FireEnd() => OnFireEnd?.Invoke();
    }
}