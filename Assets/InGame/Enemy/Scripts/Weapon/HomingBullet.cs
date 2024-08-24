using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enemy
{
    public class HomingBullet : Bullet
    {
        [Range(0, 10)]
        [SerializeField] private int _homingPower = 1;

        private Transform _player;
        private Vector3 _initial;
        private Vector3 _velocity;
        bool _isHoming;

        public override void Shoot(Vector3 direction, IOwnerTime ownerTime)
        {
            base.Shoot(direction, ownerTime);

            _player = FindPlayer();
            _initial = direction;
            _velocity = direction;
            _isHoming = true;
        }

        private static Transform FindPlayer()
        {
            return GameObject.FindGameObjectWithTag(Const.PlayerTag).transform;
        }

        protected override void StayShooting(float _)
        {
            Vector3 target = (_player.position - _transform.position).normalized;
            Vector3 forward = _velocity.normalized;

            // 90度以上だと戻ってくるような挙動をしてしまうのでホーミングを無効化。
            if (Angle(_initial, forward) >= 90.0f) _isHoming = false;

            if (_isHoming)
            {
                _velocity = Vector3.Lerp(forward, target, Time.deltaTime * _homingPower);
            }

            _transform.position += _velocity * Time.deltaTime * _speed;
        }

        private static float Angle(in Vector3 a, in Vector3 b)
        {
            float dot = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
            return Mathf.Acos(dot) * Mathf.Rad2Deg;
        }
    }
}
