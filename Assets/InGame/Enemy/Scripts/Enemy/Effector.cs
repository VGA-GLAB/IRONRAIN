using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵本体が再生するエフェクト一覧
    /// </summary>
    [System.Serializable]
    public class EnemyEffects
    {
        [SerializeField] private Effect _thruster;
        [SerializeField] private Effect _destroyed;
        [SerializeField] private Effect _trail;

        public Effect Thruster => _thruster;
        public Effect Destroyed => _destroyed;
        public Effect Trail => _trail;
    }

    /// <summary>
    /// 演出を再生する。
    /// </summary>
    public class Effector
    {
        private EnemyEffects _effects;
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
        public void PlayDestroyedEffect()
        {
            if (_effects.Destroyed == null) return;

            _effects.Destroyed.Play(_ownerTime);
        }
    }
}
