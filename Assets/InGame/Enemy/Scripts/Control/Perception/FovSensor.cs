using Enemy.DebugUse;
using Enemy.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Control
{
    /// <summary>
    /// 球状の視界を調べる。
    /// </summary>
    public class FovSensor
    {
        /// <summary>
        /// 視界に捉えた際に1度だけ呼び出される。
        /// 複数の対象を捉えた場合、捉えた対象ごとに呼び出される。
        /// </summary>
        public event UnityAction<Collider> OnCaptureEnter;
        /// <summary>
        /// 視界に捉えている間呼び出される。
        /// 複数の対象を捉えた場合、捉えた対象ごとに呼び出される。
        /// </summary>
        public event UnityAction<Collider> OnCaptureStay;
        /// <summary>
        /// 視界から外れた際に1度だけ呼び出される。
        /// 複数の対象を捉えた場合、捉えた対象ごとに呼び出される。
        /// </summary>
        public event UnityAction<Collider> OnCaptureExit;

        private Transform _transform;
        private Transform _rotate;
        private EnemyParams _params;
        // 視界に捉えたオブジェクトを前フレームと比較して各コールバックを呼ぶ。
        private HashSet<Collider> _prev;
        private HashSet<Collider> _current;

        public FovSensor(Transform transform, Transform rotate, EnemyParams enemyParams)
        {
            _transform = transform;
            _rotate = rotate;
            _params = enemyParams;
            _prev = new HashSet<Collider>();
            _current = new HashSet<Collider>();
        }

        /// <summary>
        /// 視界内を調べる
        /// </summary>
        public void CheckFOV()
        {
            _current.Clear();

            // 球状の当たり判定なので対象が上下にズレている場合は当たらない場合がある。
            RaycastExtensions.OverlapSphere(Origin(), _params.Battle.FovRadius, col =>
            {
                // 前フレームと比較するため、視界に捉えたオブジェクトを識別。
                if (col.CompareTags(Const.ViewTags)) _current.Add(col);
            });

            // 前フレームでも視界に捉えていた場合はStay、そうでなければEnterを呼ぶ。
            foreach (Collider col in _current)
            {
                if (_prev.Contains(col)) OnCaptureStay?.Invoke(col);
                else OnCaptureEnter?.Invoke(col);
            }

            // このフレームで捉えられなかった場合はExitを呼ぶ。
            foreach (Collider col in _prev)
            {
                if (!_current.Contains(col)) OnCaptureExit?.Invoke(col);
            }

            // 次のフレームで参照する
            _prev.Clear();
            foreach (Collider col in _current) _prev.Add(col);
        }

        // オフセット込みの位置
        private Vector3 Origin()
        {
            return _transform.OffsetPosition(_rotate, _params.Common.FOV.Offset);
        }

        /// <summary>
        /// 視界の描画
        /// </summary>
        public void DrawViewRange()
        {
            GizmosUtils.WireSphere(Origin(), _params.Battle.FovRadius, Color.green);
        }
    }
}
