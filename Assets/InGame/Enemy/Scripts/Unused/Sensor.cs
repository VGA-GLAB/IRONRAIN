using Enemy.Control;
using Enemy.DebugUse;
using Enemy.Extensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Unused
{
    /// <summary>
    /// 自身以外のオブジェクトを検知し干渉する。
    /// </summary>
    public class Sensor : MonoBehaviour
    {
        /// <summary>
        /// ダメージを受けた際に呼び出される。
        /// </summary>
        public event UnityAction<Collision> OnDamaged;
        /// <summary>
        /// 視界に捉えている間呼び出される。
        /// 複数の対象を捉えた場合、捉えた対象ごとに呼び出される。
        /// </summary>
        public event UnityAction<Collider> OnCapture;
        /// <summary>
        /// 視界に何も捉えていない間呼び出される。
        /// </summary>
        public event UnityAction OnUncapture;

        [Tooltip("この判定にヒットした場合はダメージを受ける")]
        [SerializeField] private Collider _damageHitBox;
        [Header("視野範囲の設定")]
        [SerializeField] private float _viewRadius = 3.0f;

        private Transform _trasnform;

        private void Awake()
        {
            _trasnform = transform;

            EnableDamageHitBox();
        }

        private void Update()
        {
            ViewRange();
        }

        // 視界内を調べる
        private void ViewRange()
        {
            // 視界に捉えた数
            int count = 0;

            // 球状の当たり判定なので対象が上下にズレている場合は当たらない場合がある。
            RaycastExtensions.OverlapSphere(_trasnform.position, _viewRadius, col => 
            {
                if (col.CompareTags(Const.ViewTags))
                {
                    OnCapture?.Invoke(col);
                    count++;
                }
            });

            // 視界に捉えた数が0の場合のイベント
            if (count == 0) OnUncapture?.Invoke();
        }

        // トリガーと接触したらダメージを受ける
        private void EnableDamageHitBox()
        {
            if (_damageHitBox == null) return;

            _damageHitBox.isTrigger = true;

            // ダメージを受けるタグで判定
            _damageHitBox.OnCollisionEnterAsObservable()
                .Where(col => col.CompareTags(Const.DamageTags))
                .Subscribe(col => OnDamaged?.Invoke(col)).AddTo(this);
        }

        private void OnDrawGizmos()
        {
            // 視界の描画
            GizmosUtils.WireSphere(transform.position, _viewRadius, new Color(0, 1, 0, 0.1f));
        }
    }
}
