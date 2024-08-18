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
        // ここに追加
    }

    /// <summary>
    /// 弾のプーリングする。
    /// </summary>
    public class BulletPool : MonoBehaviour
    {
        private struct Message
        {
            public IOwnerTime OwnerTime;
            public BulletKey Key;
            public Vector3 Muzzle;
            public Vector3 Forward;
        }

        [System.Serializable]
        private class Config
        {
            public GameObject Prefab;
            public BulletKey Key;
        }

        [Header("弾の設定")]
        [SerializeField] private Config[] _configs;
        [Tooltip("プーリングする数。攻撃速度に応じて増やす。")]
        [SerializeField] private int _poolCapacity = 4;

        private Dictionary<BulletKey, ObjectPool> _pools;

        private void Awake()
        {
            Pool();
            ReceiveMessage();
        }

        // 弾ごとにプーリングする。
        private void Pool()
        {
            if (_configs == null) return;

            _pools = new Dictionary<BulletKey, ObjectPool>(_configs.Length);
            foreach (Config value in _configs)
            {
                ObjectPool pool = new ObjectPool(value.Prefab, _poolCapacity, $"BulletPool_{value.Prefab.name}");
                _pools.Add(value.Key, pool);
            }
        }

        // 自身にメッセージングする。
        private void ReceiveMessage()
        {
            MessageBroker.Default.Receive<Message>().Subscribe(OnMessageReceived).AddTo(this);
        }

        // プールから弾を取り出し、受信したメッセージ通りに飛ばす。
        private void OnMessageReceived(Message msg)
        {
            if (!_pools.TryGetValue(msg.Key, out ObjectPool pool))
            {
                Debug.LogWarning($"弾が辞書に登録されていない: {msg.Key}");
                return;
            }
            if (!pool.TryRent(out GameObject item))
            {
                Debug.LogWarning($"弾の在庫がプールに無い: {msg.Key}");
                return;
            }
            if (!item.TryGetComponent(out Bullet bullet))
            {
                Debug.LogWarning($"弾のスクリプトがアタッチされていない: {bullet.name}");
                return;
            }

            // 弾の発射処理
            item.transform.position = msg.Muzzle;
            bullet.Shoot(msg.Forward, msg.OwnerTime);
        }

        /// <summary>
        /// プールから取り出した弾を飛ばす。
        /// </summary>
        public static void Fire(IOwnerTime ownerTime, BulletKey key, Vector3 muzzle, Vector3 forward)
        {
            MessageBroker.Default.Publish(new Message 
            {
                OwnerTime = ownerTime,
                Key = key, 
                Muzzle = muzzle, 
                Forward = forward
            });
        }
    }
}
