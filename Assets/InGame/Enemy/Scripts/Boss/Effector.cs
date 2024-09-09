using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Enemy.Boss
{
    /// <summary>
    /// ボス本体が再生するエフェクト一覧
    /// </summary>
    [System.Serializable]
    public class BossEffects
    {
        [SerializeField] private Effect _thruster;
        [SerializeField] private Effect _destroyed;
        [SerializeField] private Effect _trail;
        [SerializeField] private Effect _weaponCrash;

        public Effect Thruster => _thruster;
        public Effect Destroyed => _destroyed;
        public Effect Trail => _trail;
        public Effect WeaponCrash => _weaponCrash;
    }

    public class Effector
    {
        private BossEffects _effects;
        private IOwnerTime _ownerTime;

        public Effector(RequiredRef requiredRef)
        {
            _effects = requiredRef.Effects;
            _ownerTime = requiredRef.BlackBoard;
        }

        /// <summary>
        /// スラスターの有効化/無効化
        /// </summary>
        public void ThrusterEnable(bool value)
        {
            if (_effects.Thruster == null) return;

            if (value) _effects.Thruster.Play(_ownerTime);
            else _effects.Thruster.Stop();
        }

        /// <summary>
        /// トレイルの有効化/無効化
        /// </summary>
        public void TrailEnable(bool value)
        {
            if (_effects.Trail == null) return;

            if (value) _effects.Trail.Play(_ownerTime);
            else _effects.Trail.Stop();
        }

        /// <summary>
        /// 死亡時の演出を再生
        /// </summary>
        public void PlayDestroyed()
        {
            if (_effects.Destroyed != null) _effects.Destroyed.Play(_ownerTime);
        }

        /// <summary>
        /// QTE、鍔迫り合いの火花を再生
        /// </summary>
        public void PlayWeaponCrash()
        {
            if (_effects.WeaponCrash != null)
            {
                _effects.WeaponCrash.Play();
            }
        }
    }
}
