using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HM
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] HomingMissile _missile;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HomingMissile m = Instantiate(_missile, transform.position, Quaternion.identity);
            }
        }
    }
}
