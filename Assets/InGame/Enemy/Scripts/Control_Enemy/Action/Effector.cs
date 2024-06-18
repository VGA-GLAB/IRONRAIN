using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 再生する演出のキー
    /// </summary>
    public enum EffectKey
    {
        Dummy,
        Dying,
        Fire,
        Destroyed,
        BladeAttack,
    }

    /// <summary>
    /// 演出を再生する。
    /// </summary>
    public class Effector
    {
        private Dictionary<EffectKey, Effect> _effects;

        public Effector(Effect[] effects)
        {
            Setup(effects);
        }

        // 再生する演出を準備
        private void Setup(Effect[] effects)
        {
            _effects = new Dictionary<EffectKey, Effect>();

            if (effects == null) return;

            foreach(Effect e in effects)
            {
                if (_effects.ContainsKey(e.Key))
                {
                    Debug.LogWarning($"再生する演出が重複している: {e.Key}");
                }
                else _effects.Add(e.Key, e);
            }
        }

        /// <summary>
        /// 演出を再生。
        /// </summary>
        public void Play(EffectKey key, IOwnerTime ownerTime)
        {
            if (_effects.TryGetValue(key, out var e) && !e.IsPlaying)
            {
                e.Play(ownerTime);
            }
        }

        /// <summary>
        /// 演出を止める。
        /// </summary>
        public void Stop(EffectKey key)
        {
            if (_effects.TryGetValue(key, out var e))
            {
                e.Stop();
            }
        }
    }
}
