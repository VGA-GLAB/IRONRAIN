using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// アニメーション再生用のキー
    /// </summary>
    public enum AnimationKey
    {
        Idle,
        Left,
        Right,
        Attack,
        Broken,
    }

    /// <summary>
    /// アニメーション再生機能
    /// </summary>
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [Header("各種アニメーションの名前")]
        [SerializeField] private string _idleName = "Idle";
        [SerializeField] private string _leftName = "Left";
        [SerializeField] private string _rightName = "Right";
        [SerializeField] private string _attackName = "Attack";
        [SerializeField] private string _brokenName = "Broken";

        private int _idleHash;
        private int _leftHash;
        private int _rightHash;
        private int _attackHash;
        private int _brokenHash;

        private void Awake()
        {
            Hash();
        }

        // ハッシュ値の計算
        private void Hash()
        {
            _idleHash = Animator.StringToHash(_idleName);
            _leftHash = Animator.StringToHash(_leftName);
            _rightHash = Animator.StringToHash(_rightName);
            _attackHash = Animator.StringToHash(_attackName);
            _brokenHash = Animator.StringToHash(_brokenName);
        }

        /// <summary>
        /// アニメーションを再生する。
        /// </summary>
        public void Play(AnimationKey key)
        {
            if (_animator == null) return;

            if (key == AnimationKey.Idle) _animator.Play(_idleHash);
            if (key == AnimationKey.Left) _animator.Play(_leftHash);
            if (key == AnimationKey.Right) _animator.Play(_rightHash);
            if (key == AnimationKey.Attack) _animator.Play(_attackHash);
            if (key == AnimationKey.Broken) _animator.Play(_brokenHash);
        }
    }
}
