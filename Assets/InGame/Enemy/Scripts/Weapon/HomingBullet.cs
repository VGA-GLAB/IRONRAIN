using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class HomingBullet : Bullet
    {
        [Header("誘導の強さ")]
        [Range(0f, 1.0f)]
        [SerializeField] private float _homingPower = 0.5f;

        private Transform _player;
        private Vector3 _velocity;
        private float _period;

        public override void Shoot(Vector3 direction, IOwnerTime ownerTime)
        {
            base.Shoot(direction, ownerTime);

            _player = FindPlayer();
            _velocity = Vector3.zero;
            _period = _lifeTime;
        }

        private static Transform FindPlayer()
        {
            return GameObject.FindGameObjectWithTag(Const.PlayerTag).transform;
        }

        protected override void StayShooting(float deltaTime)
        {
            Vector3 diff = _player.position - _transform.position;
            Vector3 acc = (diff - _velocity * _period) * 2.0f / (_period * _period);

            _period -= Time.deltaTime;
            _period = Mathf.Min(0, _period);

            _velocity += acc * Time.deltaTime;
            _transform.position += _velocity * Time.deltaTime;
        }
    }
}
// https://learning.unity3d.jp/263/
