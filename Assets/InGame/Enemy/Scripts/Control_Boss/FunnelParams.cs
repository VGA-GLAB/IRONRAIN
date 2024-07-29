using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class FunnelParams : MonoBehaviour
    {
        [Min(1)]
        [Header("体力の最大値")]
        [SerializeField] private int _maxHp = 100;

        [Range(0.01f, 10.0f)]
        [Header("攻撃間隔")]
        [SerializeField] private float _fireRate = 1.0f;

        public int MaxHp => _maxHp;
        public float FireRate => _fireRate;
    }
}
