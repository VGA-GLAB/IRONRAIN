using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    #region 使い方
    // アニメーションの終了と同じフレームに登録すると意図した挙動にならない可能性がある。
    // Unityのイベント関数の順番の問題？
    // 対策として終了1フレーム手前でイベントを呼ぶなど工夫が必要。
    // AnimationがReadonlyなので、AnimationClipが梱包されている段ボールアイコンのファイルを
    // クリックしてAnimation/Eventのタブ内に直接登録する必要がある。
    // 登録するときはFunctionにメソッド名を指定すればよい。
    #endregion

    /// <summary>
    /// アニメーションイベントにフックする用のコールバックをまとめてある。
    /// このスクリプトはAnimatorと同じオブジェクトにアタッチすること。
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationEvent : MonoBehaviour
    {
        /// <summary>
        /// 遠距離攻撃は、このタイミングで弾を発射する。
        /// </summary>
        public event UnityAction OnRangeFireStart;
        /// <summary>
        /// 遠距離攻撃かつ持続的に判定が出るもの用(まだない)。
        /// </summary>
        public event UnityAction OnRangeFireEnd;
        /// <summary>
        /// 近接攻撃の判定を出すタイミング。
        /// </summary>
        public event UnityAction OnMeleeAttackStart;
        /// <summary>
        /// 近接攻撃の判定を消すタイミング。
        /// </summary>
        public event UnityAction OnMeleeAttackEnd;

        /// <summary>
        /// ボス戦、刀攻撃でボスがターンしたタイミング。
        /// </summary>
        public event UnityAction OnTurn;
        /// <summary>
        /// ボス戦、左腕破壊で振り下ろした刀がプレイヤーの左腕に直撃するタイミング。
        /// </summary>
        public event UnityAction OnBreakLeftArm;
        /// <summary>
        /// ボス戦のQTEで、プレイヤーの武器とボスの武器がぶつかった際のエフェクトを出すタイミング。
        /// </summary>
        public event UnityAction OnWeaponCrash;
        /// <summary>
        /// ボス戦のQTE、刀を展開するタイミング。
        /// </summary>
        public event UnityAction OnQteBladeOpen;
        /// <summary>
        /// 刀を振った際の音が鳴るタイミング。
        /// </summary>
        public event UnityAction OnBladeSwing;

        // アニメーションイベントに登録するメソッド群。
        // ○○○.fbxのインスペクター、AnimationタブのEvents項目に、メソッド名を記述する。
        public void RangeFireStart() => OnRangeFireStart?.Invoke();
        public void RangeFireEnd() => OnRangeFireEnd?.Invoke();
        public void MeleeAttackStart() => OnMeleeAttackStart?.Invoke();
        public void MeleeAttackEnd() => OnMeleeAttackEnd?.Invoke();
        // 以下はボス専用。
        public void Turn() => OnTurn?.Invoke();
        public void BreakLeftArm() => OnBreakLeftArm?.Invoke();
        public void WeaponCrash() => OnWeaponCrash?.Invoke();
        public void QteBladeOpen()=> OnQteBladeOpen?.Invoke();
        public void BladeSwing() => OnBladeSwing?.Invoke();
    }
}
