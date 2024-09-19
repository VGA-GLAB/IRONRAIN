using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class HomingBullet : Bullet
    {
        [Range(0, 10)]
        [SerializeField] private int _homingPower = 1;
        [SerializeField] private string _seName; 

        private Transform _player;
        private Vector3 _initial;
        private Vector3 _velocity;
        bool _isHoming;
        private int _seIndex;

        protected override void OnShoot(Vector3 direction)
        {
            _player = FindPlayer();
            _initial = direction;
            _velocity = direction;
            _isHoming = true;

            if (_seName != string.Empty)
            {
                // 発射音
                Vector3 p = transform.position;
                _seIndex = AudioWrapper.PlaySE(p, _seName);
            }
        }

        // 現状使っていないが、一応オーバーライドしておく。
        protected override void OnShoot(Transform target)
        {
            Vector3 dir = target.position - _transform.position;
            OnShoot(dir);
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

            if (_seName != string.Empty)
            {
                AudioWrapper.UpdateSePosition(transform.position, _seIndex);
            }
        }

        private static float Angle(in Vector3 a, in Vector3 b)
        {
            float dot = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
            return Mathf.Acos(dot) * Mathf.Rad2Deg;
        }
    }
}
