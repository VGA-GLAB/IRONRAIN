using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.DebugUse
{
    public class PokeEvent : MonoBehaviour
    {
        [SerializeField] Transform _enemy;

        public void ScaleUp()
        {
            _enemy.localScale = Vector3.one * 1.2f;
        }

        public void ScaleDown()
        {
            _enemy.localScale = Vector3.one;
        }
    }
}
