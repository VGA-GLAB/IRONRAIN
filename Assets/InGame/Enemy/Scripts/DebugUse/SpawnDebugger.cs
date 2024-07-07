using Enemy.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDebugger : MonoBehaviour
{
    [SerializeField] private Transform _t;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var e = FindAnyObjectByType<EnemyManager>();
            e.Spawn(_t.position, EnemyManager.Sequence.MultiBattle);
        }
    }
}
