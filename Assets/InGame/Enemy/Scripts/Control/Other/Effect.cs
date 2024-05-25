using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// キャラクター毎の再生する演出。
    /// パーティクルと音をセットで扱う。
    /// </summary>
    public class Effect : MonoBehaviour
    {
        [Header("エフェクトを管理する用のキー")]
        [SerializeField] private EffectKey _key;
        [Header("再生するパーティクル")]
        [SerializeField] private ParticleSystem[] _particles;
        [Header("再生する音")]
        [SerializeField] private string _audioName;

        // 再生する度に再生時間をリセットして計測する。
        private float _max = 0;
        private float _lifeTime = 0;

        /// <summary>
        /// 管理用のキー
        /// </summary>
        public EffectKey Key => _key;
        /// <summary>
        /// 再生中かのフラグ
        /// </summary>
        public bool IsPlaying => _lifeTime > 0;

        private void Update()
        {
            _lifeTime -= BlackBoard.DeltaTime;
            _lifeTime = Mathf.Max(0, _max);
        }

        /// <summary>
        /// 演出を再生
        /// </summary>
        public void Play()
        {
            // 音を再生する処理ｺｺ

            if (_particles == null) return;

            float d = 0;
            foreach (ParticleSystem p in _particles)
            {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
                // 自身もしくは子にループしているパーティクルが存在する場合は
                // 再生時間を極端に長くすることで、無限に再生されるように見せる。
                if (p.loop) d = float.MaxValue;
                else d = Mathf.Max(d, p.duration);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です

                p.Play();
            }

            // 一番再生時間が長いパーティクルに合わせる。
            _max = d;
        }

        /// <summary>
        /// 演出を停止
        /// </summary>
        public void Stop()
        {
            if (_particles == null) return;

            foreach (ParticleSystem p in _particles) p.Stop();
            // 再生時間を強制的に0にする。
            _lifeTime = 0;
        }
    }
}
