using Enemy.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.DebugUse
{
    public class SurroundingPoolDebugger : MonoBehaviour
    {
        [SerializeField] private SurroundingPool _pool;

        private Queue<Slot> _q = new Queue<Slot>();

        private void Update()
        {
            if (_pool == null) return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (_pool.TryRent(out Slot s)) _q.Enqueue(s);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (_q.TryDequeue(out Slot s)) _pool.Return(s);
            }

            Debug.Log($"空きスロット数:{_pool.EmptySlotCount}");
        }
    }
}
