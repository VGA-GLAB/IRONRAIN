using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    /// <summary>
    /// 敵本体が再生するエフェクト一覧
    /// </summary>
    [System.Serializable]
    public class FunnelEffects
    {
        [SerializeField] private Effect _destroyed;
        [SerializeField] private Effect _trail;

        public Effect Destroyed => _destroyed;
        public Effect Trail => _trail;
    }

    /// <summary>
    /// 演出を再生する。
    /// </summary>
    public class Effector
    {
        private FunnelEffects _effects;
        private IOwnerTime _ownerTime;

        public Effector(RequiredRef requiredRef)
        {
            _effects = requiredRef.Effects;
            _ownerTime = requiredRef.BlackBoard;
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
