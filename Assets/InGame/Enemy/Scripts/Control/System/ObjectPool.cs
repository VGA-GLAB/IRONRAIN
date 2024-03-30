using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// GameObjectのプーリング機能。
    /// オブジェクトがアクティブか否かで判定する。
    /// </summary>
    public class ObjectPool
    {
        private Transform _parent;
        private List<GameObject> _pool;

        public ObjectPool(GameObject prefab, int capacity, string name = "ObjectPool")
        {
            _parent = new GameObject(name).transform;
            _pool = new(capacity);

            for (int i = 0; i < capacity; i++)
            {
                GameObject g = Object.Instantiate(prefab);
                _pool.Add(g);
                g.transform.parent = _parent;
                g.SetActive(false);
            }
        }

        /// <summary>
        /// プールから取得。
        /// </summary>
        public GameObject Rent()
        {
            foreach (GameObject g in _pool)
            {
                if (!g.activeInHierarchy) { g.SetActive(true); return g; }
            }

            return null;
        }

        /// <summary>
        /// プールから取得。
        /// </summary>
        public bool TryRent(out GameObject item)
        {
            item = Rent();
            return item != null;
        }

        /// <summary>
        /// プールに戻す。
        /// </summary>
        public void Return(GameObject item)
        {
            item.SetActive(false);
        }
    }
}
