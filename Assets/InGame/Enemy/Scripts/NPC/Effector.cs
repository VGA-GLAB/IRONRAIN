using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    /// <summary>
    /// NPCが再生するエフェクト一覧
    /// </summary>
    [System.Serializable]
    public class NpcEffects
    {
        [SerializeField] private Effect[] _thruster;
        [SerializeField] private Effect _trail;

        public Effect[] Thruster => _thruster;
        public Effect Trail => _trail;
    }

    /// <summary>
    /// 演出を再生する。
    /// </summary>
    public class Effector
    {
        private NpcEffects _effects;
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

            foreach (Effect e in _effects.Thruster)
            {
                if (value) e.Play(_ownerTime);
                else e.Stop();
            }
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
    }
}
