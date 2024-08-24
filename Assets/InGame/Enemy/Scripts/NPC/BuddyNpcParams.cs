using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.NPC
{
    public class BuddyNpcParams : MonoBehaviour
    {
        [Header("撃破する対象")]
        [SerializeField] private EnemyController _target;

        [Header("登場するシーケンスのID")]
        [SerializeField] private int _sequenceID;

        [Header("移動速度")]
        [Min(1)]
        [SerializeField] private float _moveSpeed = 72.0f;

        [Header("目標を撃破する距離")]
        [Min(1)]
        [SerializeField] private float _defeatDistance = 5.0f;

        public EnemyController Target => _target;
        public int SequenceID => _sequenceID;
        public float MoveSpeed => _moveSpeed;
        public float DefeatDistance => _defeatDistance;
        public float DefeatSqrDistance => _defeatDistance * _defeatDistance;
        public float LifeTime = 10.0f; // 適当

        /// <summary>
        /// ギズモに描画。
        /// </summary>
        public void Draw()
        {
            // 目標までの線を描画
            if (_target != null)
            {
                Vector3 p = transform.position;
                Vector3 tp = _target.transform.position;
                GizmosUtils.Line(p, tp, ColorExtensions.ThinWhite);
            }
            
            // 撃破距離を描画
            {
                Vector3 p = transform.position;
                GizmosUtils.WireCircle(p, DefeatDistance, ColorExtensions.ThinRed);
            }
        }
    }
}
