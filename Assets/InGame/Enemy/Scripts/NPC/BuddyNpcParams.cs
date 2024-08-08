using UnityEngine;

namespace Enemy.NPC
{
    public class BuddyNpcParams : MonoBehaviour
    {
        [Header("移動速度")]
        [SerializeField] private float _moveSpeed = 72.0f;

        [Header("目標を撃破する距離")]
        [Min(1)]
        [SerializeField] private float _defeatDistance = 5.0f;

        public float MoveSpeed => _moveSpeed;
        public float DefeatDistance => _defeatDistance;
        public float DefeatSqrDistance => _defeatDistance * _defeatDistance;
    }
}
