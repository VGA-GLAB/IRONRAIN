using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace Enemy.Boss
{
    /// <summary>
    /// ボス本体が再生するエフェクト一覧
    /// </summary>
    [System.Serializable]
    public class BossEffects
    {
        [SerializeField] private Effect _thruster;
        [SerializeField] private Effect[] _destroyed;
        [SerializeField] private Effect _trail;
        [SerializeField] private Effect _weaponCrash;

        public Effect Thruster => _thruster;
        public Effect[] Destroyed => _destroyed;
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

            Ref = requiredRef;
        }

        private RequiredRef Ref { get; }

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
            if (_effects.Destroyed != null)
            {
                Ref.Transform.GetComponent<BossController>().StartCoroutine(PlayDestroyedAsync());
            }
        }

        private IEnumerator PlayDestroyedAsync()
        {
            for (int i = 0; i < _effects.Destroyed.Length - 1; i++)
            {
                _effects.Destroyed[i].Play(_ownerTime);
                Vector3 p = Ref.Body.Position;
                AudioWrapper.PlaySE(p, "SE_Kill");
                
                yield return new WaitForSeconds(0.33f);
            }

            // 最後のものだけ遅延して鳴らす。
            yield return new WaitForSeconds(0.67f);
            _effects.Destroyed[^1].Play();
            Vector3 q = Ref.Body.Position;
            AudioWrapper.PlaySE(q, "SE_Kill");
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
