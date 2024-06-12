using Enemy.DebugUse;
using Enemy.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Control
{
    /// <summary>
    /// 球状の視界を調べる。
    /// 有効化後、毎フレーム視界内を調べる度に状況によってコールバックが呼ばれる。
    /// </summary>
    public class EyeSensor
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
        private EnemyParams _params;
        private BlackBoard _blackBoard;
        private Transform _rotate;

        // 視界に捉えたオブジェクトを前フレームと比較して各コールバックを呼ぶ。
        private HashSet<Collider> _prev;
        private HashSet<Collider> _current;

        public EyeSensor(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard, Transform rotate)
        {
            _transform = transform;
            _params = enemyParams;
            _blackBoard = blackBoard;
            _rotate = rotate;
            _prev = new HashSet<Collider>();
            _current = new HashSet<Collider>();
        }

        /// <summary>
        /// 有効化
        /// </summary>
        public void Enable()
        {
            OnCaptureEnter += CaptureEnter;
            OnCaptureStay += CaptureStay;
            OnCaptureExit += CaptureExit;
        }

        /// <summary>
        /// 無効化
        /// </summary>
        public void Disable()
        {
            OnCaptureEnter -= CaptureEnter;
            OnCaptureStay -= CaptureStay;
            OnCaptureExit -= CaptureExit;
        }

        // 視界センサーのコールバックに登録するメソッドたち。
        private void CaptureEnter(Collider collider) => _blackBoard.FovEnter.Add(collider);
        private void CaptureStay(Collider collider) => _blackBoard.FovStay.Add(collider);
        private void CaptureExit(Collider collider) => _blackBoard.FovExit.Add(collider);

        /// <summary>
        /// 視界内を調べる。
        /// </summary>
        public void Update()
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

        /// <summary>
        /// 黒板に書き込んだ内容を全て消す。
        /// </summary>
        public void ClearCaptureTargets()
        {
            _blackBoard.FovEnter.Clear();
            _blackBoard.FovStay.Clear();
            _blackBoard.FovExit.Clear();
        }

        // 視界の原点。
        private Vector3 Origin()
        {
            Vector3 ox = _rotate.right * _params.Common.FOV.Offset.x;
            Vector3 oy = _rotate.up * _params.Common.FOV.Offset.y;
            Vector3 oz = _rotate.forward * _params.Common.FOV.Offset.z;
            return _transform.position + ox + oy + oz;
        }

        /// <summary>
        /// 視界の描画
        /// </summary>
        public void Draw()
        {
            GizmosUtils.WireSphere(Origin(), _params.Battle.FovRadius, Color.green);
        }
    }
}
