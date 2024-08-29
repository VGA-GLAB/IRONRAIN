using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class TranslatedBullet : Bullet
    {
        private Vector3 _direction;

        protected override void OnShoot(Vector3 direction)
        {
            _direction = direction;
        }

        protected override void OnShoot(Transform target) 
        {
            Vector3 dir = target.position - _transform.position;
            OnShoot(dir);
        }
        
        protected override void StayShooting(float deltaTime)
        {
            _transform.position += _direction * deltaTime * _speed;
        }
    }
}
