using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 球状の視界を調べる。
    /// 有効化後、毎フレーム視界内を調べる度に状況によってコールバックが呼ばれる。
    /// </summary>
    public class EyeSensor
    {
        // プレイヤー + 自身以外の敵が最大4体。
        private const int Capacity = 5;

        private Transform _transform;
        private EnemyParams _params;
        private BlackBoard _blackBoard;
        private Transform _rotate;

        private Collider[] _result;

        public EyeSensor(RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;
            _rotate = requiredRef.Rotate;
            _result = new Collider[Capacity];
        }

        /// <summary>
        /// 視界内を調べる。
        /// </summary>
        public void Update()
        {
            if (_blackBoard.CurrentState == StateKey.Hide) return;

            // とりあえずプレイヤーのみ検知で十分。
            const int PlayerLayer = 1 << 6;
            if (Physics.OverlapSphereNonAlloc(Origin(), _params.FovRadius, _result, PlayerLayer) > 0)
            {
                foreach (Collider col in _result)
                {
                    if (col == null) break;
                    else _blackBoard.FovStay.Add(col);
                }
            }
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
        /// 黒板に書き込んだ内容を全て消す。
        /// </summary>
        public void ClearCaptureTargets()
        {
            // フレームを跨ぐ前に消しているので、ギズモへの描画が難しい。
            _blackBoard.FovStay.Clear();
        }

        /// <summary>
        /// 視界の描画
        /// </summary>
        public void Draw()
        {
            GizmosUtils.WireCircle(Origin(), _params.FovRadius, ColorExtensions.ThinGreen);
        }
    }
}
