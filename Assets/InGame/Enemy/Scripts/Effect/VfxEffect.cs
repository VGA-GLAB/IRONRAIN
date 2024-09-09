using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Enemy
{
    public class VfxEffect : Effect
    {
        private VisualEffect[] _all;

        public override EffectType Type => EffectType.VFX;

        protected override void OnAwake()
        {
            _all = GetComponentsInChildren<VisualEffect>();

            // 呼び出されるまで無効化しておく。
            Stop();
        }

        protected override void OnStart()
        {
        }

        protected override void OnPlay(IOwnerTime _)
        {
            foreach (VisualEffect e in _all) e.enabled = true;
        }

        protected override void OnStop()
        {
            foreach (VisualEffect e in _all) e.enabled = false;
        }

        protected override IEnumerator OnPlayAsync()
        {
            // 本来はシリアライズする必要ありだが、このメソッド自体が仕様が決まるまでの限定的なものなので、定数で十分？
            const float PlayTime = 1.0f;

            Play(null); // 現状IOwnerTimeを使用していないのでnullで大丈夫。
            yield return new WaitForSeconds(PlayTime);
            Stop();
        }
    }
}
