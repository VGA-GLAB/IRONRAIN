using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// ボス戦QTEでアニメーションの特定のタイミングに合わせる
    /// 必要がある演出などをAnimatinoEventに登録する。
    /// </summary>
    public class QteAnimationEvent : MonoBehaviour
    {
        [Header("アニメーションイベントに処理をフック")]
        [SerializeField] private AnimationEvent _animationEvent;
        [Header("再生するエフェクト")]
        [SerializeField] private Effect _weaponCrashEffect;

        private IOwnerTime _owner;

        private void Start()
        {
            if (TryGetComponent(out BossController b)) _owner = b.BlackBoard;
        }

        private void OnEnable()
        {
            _animationEvent.OnWeaponCrash += PlayWeaponCrashEffect;
            _animationEvent.OnWeaponCrash += StopWeaponCrashEffect;
        }

        private void OnDisable()
        {
            _animationEvent.OnWeaponCrash -= PlayWeaponCrashEffect;
            _animationEvent.OnWeaponCrash -= StopWeaponCrashEffect;
        }

        /// <summary>
        /// プレイヤーとボスの武器がぶつかった際の演出を再生。
        /// </summary>
        private void PlayWeaponCrashEffect()
        {
            if (_weaponCrashEffect != null)
            {
                _weaponCrashEffect.PlayAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }

        /// <summary>
        /// プレイヤーとボスの武器がぶつかった際の演出を停止。
        /// </summary>
        private void StopWeaponCrashEffect()
        {
            // アニメーション側の不具合？により、アニメーション全体の
            // 最初の3~4フレームまでしかアニメーションイベントが再生されないので
            // 一旦使わないでおいておく。
        }
    }
}
