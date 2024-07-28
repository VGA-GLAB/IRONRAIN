using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
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

        public Effect Thruster => _thruster;
        public Effect Destroyed => _destroyed;
        public Effect Trail => _trail;
    }

    public class Effector
    {
        private BossEffects _effects;
        private IOwnerTime _ownerTime;

        public Effector(BossEffects effects, IOwnerTime ownerTime)
        {
            _effects = effects;
            _ownerTime = ownerTime;
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
        public void PlayDestroyedEffect()
        {
            if (_effects.Destroyed == null) return;

            _effects.Destroyed.Play(_ownerTime);
        }
    }
}
