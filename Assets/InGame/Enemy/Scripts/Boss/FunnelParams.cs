using UnityEngine;

namespace Enemy.Boss
{
    public class FunnelParams : MonoBehaviour
    {
        [Min(1)]
        [Header("体力の最大値")]
        [SerializeField] private int _maxHp = 100;

        [Range(0.01f, 10.0f)]
        [Header("攻撃間隔")]
        [SerializeField] private float _fireRate = 1.0f;

        [Range(0, 1.0f)]
        [Header("攻撃の精度")]
        [Tooltip("値を大きくすると、プレイヤーの正面に弾が飛んでくる確率が上がる。")]
        [SerializeField] private float _accuracy;

        [Range(5.0f, 20.0f)]
        [Header("ボスを追跡する速さ")]
        [SerializeField] private float _moveSpeed;

        public int MaxHp => _maxHp;
        public float FireRate => _fireRate;
        public float Accuracy => _accuracy;
        public float MoveSpeed => _moveSpeed;
    }
}
