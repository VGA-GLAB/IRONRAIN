using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 高速で移動しているプレイヤーに向けて撃つ場合、Translateで移動させると一瞬で突き抜けていく。
    /// そこで、相対的な位置をLerpで移動させる。
    /// </summary>
    public class LerpedBullet : Bullet
    {
        private Vector3 _start;
        private Transform _target;
        private float _lerp;
        private float _diff;
        private float _targetX;

        protected override void OnShoot(Vector3 direction)
        {
            Debug.LogWarning($"{nameof(LerpedBullet)}:この方法では撃てない。");
        }

        protected override void OnShoot(Transform target)
        {
            _start = _transform.position - target.position;
            _target = target;
            _lerp = 0;
            _diff = _start.magnitude;
            _targetX = target.position.x;
        }

        protected override void StayShooting(float deltaTime)
        {
            Vector3 l = Vector3.Lerp(_start, Vector3.zero, _lerp);
            Vector3 p = _target.position;
            p.x = _targetX;
            _transform.position = p + l;

            _lerp += _speed / _diff * deltaTime;
            _lerp = Mathf.Clamp01(_lerp);
        }
    }
}
