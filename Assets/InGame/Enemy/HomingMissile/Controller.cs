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
                for (int i = 0; i < 10; i++) Fire();
            }
        }

        void Fire()
        {
            HomingMissile m = Instantiate(_missile, transform.position, Quaternion.identity);
            Transform boss = GameObject.Find("Boss").transform;
            Transform[] funnels = boss.GetComponentsInChildren<Transform>();
            Transform target = funnels[Random.Range(0, funnels.Length)];
            float x = Random.value;
            float y = Random.value;
            float z = Random.value;
            Vector3 launch = new Vector3(x, y, z);
            m.Fire(target, launch);
        }
    }
}
