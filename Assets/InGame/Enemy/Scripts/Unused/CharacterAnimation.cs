using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Enemy.Unused
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
        [System.Serializable]
        class Data
        {
            public string Name;
            public float PlayTime;
        }

        [SerializeField] private Animator _animator;
        [Header("各種アニメーションの名前")]
        [SerializeField] private Data _idle;
        [SerializeField] private Data _left;
        [SerializeField] private Data _right;
        [SerializeField] private Data _attack;
        [SerializeField] private Data _broken;

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
            _idleHash = Animator.StringToHash(_idle.Name);
            _leftHash = Animator.StringToHash(_left.Name);
            _rightHash = Animator.StringToHash(_right.Name);
            _attackHash = Animator.StringToHash(_attack.Name);
            _brokenHash = Animator.StringToHash(_broken.Name);
        }

        /// <summary>
        /// アニメーションを再生し、終了まで待つ。
        /// </summary>
        public async UniTask PlayAsync(AnimationKey key, CancellationToken token)
        {
            if (_animator == null) return;

            int hash = _idleHash;
            float playTime = _idle.PlayTime;
            if (key == AnimationKey.Left) { hash = _leftHash; playTime = _left.PlayTime; }
            if (key == AnimationKey.Right) { hash = _rightHash; playTime = _right.PlayTime; }
            if (key == AnimationKey.Attack) { hash = _attackHash; playTime = _attack.PlayTime; }
            if (key == AnimationKey.Broken) { hash = _brokenHash; playTime = _broken.PlayTime; }

            _animator.Play(hash);

            await UniTask.WaitForSeconds(playTime, cancellationToken: token);
        }
    }
}
