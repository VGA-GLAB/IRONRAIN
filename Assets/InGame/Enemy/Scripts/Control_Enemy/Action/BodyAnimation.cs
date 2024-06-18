using Enemy.Control.FSM;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Control
{
    #region 注意書き
    // ObservableStateMachineTriggerのコールバックは遷移元Exit→遷移先Enterの順番で呼ばれない。
    // 遷移先Enterの数フレーム後に遷移元Exitが呼ばれる。滑らかなアニメーションの遷移をするため？
    // Layer名からclip名を取得して再生中か調べる方法は、clip名をステート名と揃えるか、
    // clip名の定数のクラスを作る必要があり、これ以上複雑化して管理できなくなりそうなので断念。
    #endregion

    /// <summary>
    /// アニメーションさせる。
    /// ObservableStateMachineTriggerコンポーネントがAnimatorにアタッチされている必要がある。
    /// 列挙型ではなく定数値の文字列で管理する。
    /// </summary>
    public class BodyAnimation
    {
        // ボスを除く3種類の雑魚敵のステート名の定数。
        // 敵の種類ごとにそれぞれ別の名前かつ個数もまばらなので分ける。
        public static class StateName
        {
            public static class MachineGun
            {
                public const string Idle = "Idle";
                public const string MoveFrontLoop = "all_move_front_lp";
                public const string MoveFrontEnd = "all_move_front_ed";
                public const string MoveLeftStart = "all_move_left_st";
                public const string MoveLeftLoop = "all_move_left_lp";
                public const string MoveLeftEnd = "all_move_left_ed";
                public const string MoveRightStart = "all_move_right_st";
                public const string MoveRightLoop = "all_move_right_lp";
                public const string MoveRightEnd = "all_move_right_ed";
                public const string HoldStart = "enemy_assault_hold_st"; // 武器構え
                public const string HoldLoop = "enemy_assault_hold_lp";  // 武器構え
                public const string FireLoop = "enemy_assault_fire_lp";  // 攻撃
                public const string Damage = "enemy_break00";
            }

            public static class Launcher
            {
                public const string Idle = "Idle";
                public const string MoveFrontLoop = "all_move_front_lp";
                public const string MoveFrontEnd = "all_move_front_ed";
                public const string MoveLeftStart = "all_move_left_st";
                public const string MoveLeftLoop = "all_move_left_lp";
                public const string MoveLeftEnd = "all_move_left_ed";
                public const string MoveRightStart = "all_move_right_st";
                public const string MoveRightLoop = "all_move_right_lp";
                public const string MoveRightEnd = "all_move_right_ed";
                public const string HoldStart = "enemy_rocket_hold_st"; // 武器構え
                public const string HoldLoop = "enemy_rocket_hold_lp";  // 武器構え
                public const string Fire = "enemy_rocket_fire";         // 攻撃(アニメーション3分割なし)
                public const string Reload = "enemy_rocket_reload";     // 武器をリロード
                public const string Damage = "enemy_break00";
            }

            public static class Shield
            {
                public const string Idle = "Idle";
                public const string MoveFrontLoop = "all_move_front_lp";
                public const string MoveFrontEnd = "all_move_front_ed";
                public const string MoveLeftStart = "all_move_left_st";
                public const string MoveLeftLoop = "all_move_left_lp";
                public const string MoveLeftEnd = "all_move_left_ed";
                public const string MoveRightStart = "all_move_right_st";
                public const string MoveRightLoop = "all_move_right_lp";
                public const string MoveRightEnd = "all_move_right_ed";
                public const string ShieldStart = "enemy_shield_st";  // 盾構え
                public const string ShieldLoop = "enemy_shield_lp";   // 盾構え
                public const string Attack = "enemy_shield_attack00"; // 攻撃？
                public const string Damage = "enemy_break01";
            }

            public static class Boss
            {
                public const string Idle = "Idle";
                public const string MoveFrontLoop = "all_move_front_lp";
                public const string MoveFrontEnd = "all_move_front_ed";
                public const string MoveLeftStart = "all_move_left_st";
                public const string MoveLeftLoop = "all_move_left_lp";
                public const string MoveLeftEnd = "all_move_left_ed";
                public const string MoveRightStart = "all_move_right_st";
                public const string MoveRightLoop = "all_move_right_lp";
                public const string MoveRightEnd = "all_move_right_ed";
                public const string HoldStart = "enemy_assault_hold_st";   // 銃構え
                public const string HoldLoop = "enemy_assault_hold_lp";    // 銃構え
                public const string FireLoop = "enemy_assault_fire_lp";    // 銃攻撃
                public const string BladeStart = "enemy_shield_st";        // 刀構え
                public const string BladeLoop = "enemy_shield_lp";         // 刀構え
                public const string BladeAttack = "enemy_shield_attack00"; // 刀攻撃
                public const string Damage = "enemy_break00";
            }
        }

        // ボスを含む敵のパラメータ名の定数。
        public static class ParamName
        {
            // この値をAnimatorのステートのSpeedに乗算した値が最終的なアニメーション再生速度。
            public const string PlaySpeed = "PlaySpeed";

            // アイドルのBlendTreeの制御に使われており、基本値は1。
            public const string SpeedY = "SpeedY";

            // プレイヤーを検知~接近が完了したタイミングをトリガー。
            public const string ApproachEndTrigger = "FinishFirstMoveTrigger";

            // 接近が完了し、戦闘状態で使用。
            // trueでアイドルから右に移動するアニメーションを開始。
            // falseで右に移動するアニメーションを終了し、アイドル状態に戻る。
            public const string IsRightMove = "IsRightMove";

            // 接近が完了し、戦闘状態で使用。
            // trueでアイドルから左に移動するアニメーションを開始。
            // falseで左に移動するアニメーションを終了し、アイドル状態に戻る。
            public const string IsLeftMove = "IsLeftMove";

            // 接近が完了し、戦闘状態で使用。
            // 武器を構える。
            public const string AttackSetTrigger = "AttackSetTrigger";

            // AttackSetTriggerで武器を構えた状態が前提。
            // 武器で攻撃する。
            public const string AttackTrigger = "AttackTrigger";

            // AttackTriggerで武器で攻撃している状態が前提。
            // 武器を下げてアイドル状態に戻る。
            public const string AttackEndTrigger = "AttackEndTrigger";

            // どのアニメーションからでも遷移可能。
            // ダメージを受ける。
            public const string GetDamageTrigger = "GetDamageTrigger";

            // どのアニメーションからでも遷移可能。
            // プレイヤーがQTE成功。
            // 盾持ちはGetDamageTriggerではなくこちらを使う。
            public const string BreakTrigger = "BreakTrigger";

            // ボスの登場が完了し、戦闘状態で使用。
            // ボスが刀を構える。
            public const string TempBladeAttackSetTrigger = "TempBladeAttackSetTrigger";

            // TempBladeAttackSetTriggerで刀を構えた状態が前提。
            // ボスが刀で攻撃する。
            public const string TempBladeAttackTrigger = "TempBladeAttackTrigger";
        }

        // Animatorの各ステート
        private class State
        {
            // 再生開始時のコールバック
            public UnityAction OnPlayEnter;
            // 再生終了時のコールバック
            public UnityAction OnPlayExit;

            public State(string s)
            {
                Name = s;
                Hash = Animator.StringToHash(s);
                OnPlayEnter = null;
                OnPlayExit = null;
            }

            public string Name { get; private set; }
            public int Hash { get; private set; }
        }

        private Animator _animator;
        // 文字列の定数値で各ステートを管理する。
        private Dictionary<string, State> _stateTable;
        // コールバックの解除を行うために、登録した匿名関数を保持しておく。
        // 身体側のステート毎に登録解除出来る。
        private Dictionary<StateKey, List<UnityAction>> _callbacks;

        public BodyAnimation(Animator animator)
        {
            _animator = animator;
            _stateTable = new Dictionary<string, State>();
            _callbacks = new Dictionary<StateKey, List<UnityAction>>();

            StateMachineTriggerCallback(_animator.gameObject);
        }

        // コールバックを登録しているステートをトリガー出来るように設定する。
        private void StateMachineTriggerCallback(GameObject owner)
        {
            foreach (ObservableStateMachineTrigger trigger in _animator.GetBehaviours<ObservableStateMachineTrigger>())
            {
                // ステート開始のタイミングをトリガー。
                trigger.OnStateEnterAsObservable().Subscribe(a => OnStateEnter(a.StateInfo)).AddTo(owner);

                // ステート終了のタイミングをトリガー。
                trigger.OnStateExitAsObservable().Subscribe(a => OnStateExit(a.StateInfo)).AddTo(owner);
            }

            // ステート開始のコールバックを呼ぶ。
            void OnStateEnter(AnimatorStateInfo info)
            {
                foreach (KeyValuePair<string, State> p in _stateTable)
                {
                    // ハッシュ値でどのステートかを判定してコールバックを呼ぶ。
                    if (p.Value.Hash == info.shortNameHash) { p.Value.OnPlayEnter?.Invoke(); return; }
                }
            }

            // ステート終了のコールバックを呼ぶ。
            void OnStateExit(AnimatorStateInfo info)
            {
                foreach (KeyValuePair<string, State> p in _stateTable)
                {
                    // ハッシュ値でどのステートかを判定してコールバックを呼ぶ。
                    if (p.Value.Hash == info.shortNameHash) { p.Value.OnPlayExit?.Invoke(); return; }
                }
            }
        }

        // 辞書から指定したステートを取得。
        // 登録されていなければ、新しく登録して返す。
        private State GetState(string stateName)
        {
            if (_stateTable.TryGetValue(stateName, out State state))
            {
                return state;
            }
            else
            {
                _stateTable.Add(stateName, new State(stateName));
                return _stateTable[stateName];
            }
        }

        // コールバックを身体側のステート毎に保持しておき、一括で登録解除できるようにする。
        // ステートマシンの破棄時に身体側のステートごとに一括解除するため、EnterかExitかの区別はしない。
        private void AddCallback(StateKey key, UnityAction callback)
        {
            if (!_callbacks.ContainsKey(key))
            {
                _callbacks.Add(key, new List<UnityAction>());
            }

            _callbacks[key].Add(callback);
        }

        /// <summary>
        /// ステートを指定してアニメーションを再生。
        /// </summary>
        public void Play(string stateName)
        {
            _animator.Play(GetState(stateName).Hash);
        }

        /// <summary>
        /// ステート開始時のコールバックを登録。
        /// </summary>
        public void RegisterStateEnterCallback(StateKey key, string stateName, UnityAction callback)
        {
            GetState(stateName).OnPlayEnter += callback;
            AddCallback(key, callback);
        }

        /// <summary>
        /// ステート終了時のコールバックを登録。
        /// </summary>
        public void RegisterStateExitCallback(StateKey key, string stateName, UnityAction callback)
        {
            GetState(stateName).OnPlayExit += callback;
            AddCallback(key, callback);
        }

        /// <summary>
        /// ステートの遷移に登録したコールバックを解除する。
        /// EnterとExitの区別はせず、身体側のステート毎に一括で解除する。
        /// </summary>
        public void ReleaseStateCallback(StateKey key)
        {
            // どのステートに対してコールバックを登録したかは保持していない。
            // 全てのステートに対して総当たりで解除を試みる。
            foreach (State s in _stateTable.Values)
            {
                foreach (UnityAction a in _callbacks[key])
                {
                    if (s.OnPlayEnter != null) { s.OnPlayEnter -= a; }
                    if (s.OnPlayExit != null) { s.OnPlayExit -= a; }
                }
            }
        }

        /// <summary>
        /// アニメーションのfloat型パラメータを設定する。
        /// </summary>
        public void SetFloat(string name, float value)
        {
            _animator.SetFloat(name, value);
        }

        /// <summary>
        /// アニメーションのbool型パラメータを設定する。
        /// </summary>
        public void SetBool(string name, bool value)
        {
            _animator.SetBool(name, value);
        }

        /// <summary>
        /// アニメーションのトリガーを設定する。
        /// </summary>
        public void SetTrigger(string name)
        {
            _animator.SetTrigger(name);
        }

        /// <summary>
        /// アニメーションのトリガーをリセットする。
        /// </summary>
        public void ResetTrigger(string name)
        {
            _animator.ResetTrigger(name);
        }
    }
}