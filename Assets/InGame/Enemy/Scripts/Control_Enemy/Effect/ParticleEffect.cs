using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    public class ParticleEffect : Effect
    {
        private ParticleSystem[] _all;

        public override EffectType Type => EffectType.Particle;

        protected override void OnAwake()
        {
            _all = GetComponentsInChildren<ParticleSystem>();

            // 呼び出されるまで無効化しておく。
            Stop();
        }

        protected override void OnStart()
        {
        }

        protected override void OnPlay(IOwnerTime _)
        {
            foreach (ParticleSystem p in _all) p.Play();
        }

        protected override void OnStop()
        {
            foreach (ParticleSystem p in _all) p.Stop();
        }
    }
}
