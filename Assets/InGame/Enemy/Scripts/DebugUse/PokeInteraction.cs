using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.DebugUse
{
    public class PokeInteraction : MonoBehaviour
    {
        private void Start()
        {
            AgentScript a = GetComponent<AgentScript>();
            a.EnemyGenerate();
        }

        private void OnDestroy()
        {
            AgentScript a = GetComponent<AgentScript>();
            a.EnemyDestory();
        }
    }
}
