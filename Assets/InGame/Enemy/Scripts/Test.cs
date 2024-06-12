using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    public class Test : MonoBehaviour
    {
        [SerializeField] EnemyController _enemy;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _enemy.Attack();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _enemy.Pause();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _enemy.Resume();
            }
        }
    }
}
