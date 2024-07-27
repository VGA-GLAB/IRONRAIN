using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// エフェクトの種類
    /// </summary>
    public enum EffectType { Particle, VFX, }

    /// <summary>
    /// パーティクルとVFXの違いを吸収し、統一した再生方法を提供するための基底クラス。
    /// </summary>
    public abstract class Effect : MonoBehaviour
    {
        /// <summary>
        /// 管理する側でエフェクトの種類を判定する場合に使用。
        /// </summary>
        public abstract EffectType Type { get; }

        /// <summary>
        /// 再生中かどうかのフラグ。
        /// </summary>
        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        protected abstract void OnAwake();
        protected abstract void OnStart();

        /// <summary>
        /// 演出を再生する。
        /// 自身を再生したオブジェクトの時間の流れに合わせる。
        /// </summary>
        public void Play(IOwnerTime ownerTime)
        {
            IsPlaying = true;
            OnPlay(ownerTime);
        }
        
        /// <summary>
        /// 演出を停止。
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
            OnStop();
        }

        /// <summary>
        /// 演出を再生する。
        /// ポーズやスロー演出に対応していないので注意。
        /// </summary>
        public async UniTaskVoid PlayAsync(CancellationToken token)
        {
            IsPlaying = true;
            await OnPlayAsync(token);
            IsPlaying = false;
        }

        protected abstract void OnPlay(IOwnerTime ownerTime);
        protected abstract void OnStop();
        protected virtual async UniTask OnPlayAsync(CancellationToken token) { await UniTask.Yield(token); }
    }
}
