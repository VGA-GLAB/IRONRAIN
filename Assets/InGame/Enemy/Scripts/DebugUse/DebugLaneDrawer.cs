using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.DebugUse
{
    /// <summary>
    /// レーンをギズモ上に描画
    /// </summary>
    public class DebugLaneDrawer : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset;
        [SerializeField] private int _quantity = 5;
        [SerializeField] private float _space = 10.0f;

        private void OnDrawGizmos()
        {
            Vector3 center = Vector3.zero + _offset;
            for (int i = 0; i < _quantity; i++)
            {
                Vector3 p = center + (Vector3.right * i * _space) + (Vector3.left * _quantity / 2 * _space);
                if (_quantity % 2 != 0) p += Vector3.left * (_space / 2);

                Vector3 q = new Vector3(p.x, p.y, -1000); // zは適当
                Vector3 r = new Vector3(p.x, p.y, 1000); // zは適当

                Gizmos.color = Color.black;
                Gizmos.DrawLine(q, r);
            }
        }
    }
}
