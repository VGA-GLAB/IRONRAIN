using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy
{
    public class EyeSensor
    {
        // プレイヤー + 自身以外の敵が最大4体。
        private const int Capacity = 5;

        private Collider[] _result;

        public EyeSensor(RequiredRef requiredRef)
        {
            _result = new Collider[Capacity];
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        /// <summary>
        /// 球状の視界を調べ、捉えた対象を黒板に書き込む。
        /// </summary>
        public void Update()
        {
            Ref.BlackBoard.FovStay.Clear();

            // とりあえずプレイヤーのみ検知で十分。
            const int PlayerLayer = 1 << 6;

            float radius = Ref.EnemyParams.FovRadius;
            if (Physics.OverlapSphereNonAlloc(Origin(), radius, _result, PlayerLayer) > 0)
            {
                foreach (Collider col in _result)
                {
                    if (col == null) break;
                    else Ref.BlackBoard.FovStay.Add(col);
                }
            }
        }

        /// <summary>
        /// 視界を描画。
        /// </summary>
        public void Draw()
        {
            float radius = Ref.EnemyParams.FovRadius;
            GizmosUtils.WireCircle(Origin(), radius, ColorExtensions.ThinGreen);
        }

        // 視界の原点。
        private Vector3 Origin()
        {
            Transform rotate = Ref.Rotate;
            Vector3 offset = Ref.EnemyParams.Common.FOV.Offset;

            Vector3 ox = rotate.right * offset.x;
            Vector3 oy = rotate.up * offset.y;
            Vector3 oz = rotate.forward * offset.z;
            return Ref.Transform.position + ox + oy + oz;
        }
    }
}
