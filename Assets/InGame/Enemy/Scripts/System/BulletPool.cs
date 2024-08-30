using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 弾の種類を表すキー
    /// </summary>
    public enum BulletKey
    {
        Dummy,
        Assault,
        Launcher,
        Funnel,
        BossLauncher,
        TutorialAssault,
        TutorialLauncher,
        // ここに追加
    }

    /// <summary>
    /// 弾のプーリングする。
    /// </summary>
    public class BulletPool : MonoBehaviour
    {
        [System.Serializable]
        private class Config
        {
            public GameObject Prefab;
            public BulletKey Key;
            public int Capacity = 4;
        }

        [Header("弾の設定")]
        [SerializeField] private Config[] _configs;

        private Dictionary<BulletKey, ObjectPool> _pools;

        private void Awake()
        {
            Pool();
        }

        // 弾ごとにプーリングする。
        private void Pool()
        {
            if (_configs == null) return;

            _pools = new Dictionary<BulletKey, ObjectPool>(_configs.Length);
            foreach (Config value in _configs)
            {
                ObjectPool pool = new ObjectPool(value.Prefab, value.Capacity, $"BulletPool_{value.Prefab.name}");
                _pools.Add(value.Key, pool);
            }
        }

        /// <summary>
        /// タグで取得して返す。
        /// </summary>
        public static BulletPool Find()
        {
            GameObject g = GameObject.FindGameObjectWithTag(Const.EnemySystemTag);
            return g.GetComponent<BulletPool>();
        }

        /// <summary>
        /// プールから弾を取り出す。
        /// </summary>
        public bool TryRent(BulletKey key, out Bullet bullet)
        {
            bullet = null;

            if (!_pools.TryGetValue(key, out ObjectPool pool))
            {
                Debug.LogWarning($"弾が辞書に登録されていない: {key}");
                return false;
            }
            if (!pool.TryRent(out GameObject item))
            {
                Debug.LogWarning($"弾の在庫がプールに無い: {key}");
                return false;
            }
            if (!item.TryGetComponent(out bullet))
            {
                Debug.LogWarning($"弾のスクリプトがアタッチされていない: {bullet.name}");
                return false;
            }

            return true;
        }
    }
}
