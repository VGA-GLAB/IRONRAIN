using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Assertions;

namespace IronRain.Player
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private GameObject _assaultPrefab;
        [SerializeField] private GameObject _rocketPrefab;
        [SerializeField] private Transform _assaultShotPos;
        [SerializeField] private Transform _rocketShotPos;
        private ObjectPool<BulletCon> _assaultRiflePool;
        private ObjectPool<BulletCon> _rocketPool;

        private Transform _bulletPoolParent;

        private void Start()
        {
            var obj = new GameObject();
            obj.name = "BulletPool";
            _bulletPoolParent = Instantiate(obj).transform;
            _assaultRiflePool = new ObjectPool<BulletCon>(
                createFunc: () => InsObj(PlayerWeaponType.AssaultRifle),
                actionOnGet: x => OnGetObj(x,PlayerWeaponType.AssaultRifle),
                actionOnDestroy: x => DestroyPoolBullet(x),
                maxSize: 30
                );
            _rocketPool = new ObjectPool<BulletCon>(
                createFunc: () => InsObj(PlayerWeaponType.RocketLauncher),
                actionOnGet: x => OnGetObj(x, PlayerWeaponType.RocketLauncher),
                actionOnDestroy: x => DestroyPoolBullet(x),
                maxSize: 10
                );
        }

        public BulletCon GetBullet(PlayerWeaponType weaponType)
        {
            if (weaponType == PlayerWeaponType.AssaultRifle)
            {
                return _assaultRiflePool.Get();
            }
            else if (weaponType == PlayerWeaponType.RocketLauncher)
            {
                return _rocketPool.Get();
            }
            else 
            {
                return null;
            }
        }

        public void ReleaseBullet(BulletCon bulletCon)
        {
            bulletCon.SetVisible(false);
            _assaultRiflePool.Release(bulletCon);
        }

        private BulletCon InsObj(PlayerWeaponType playerWeaponType)
        {
            BulletCon bulletCon = null;
            if (playerWeaponType == PlayerWeaponType.AssaultRifle)
            {
                bulletCon = Instantiate(_assaultPrefab, _bulletPoolParent).GetComponent<BulletCon>();
            }
            else if (playerWeaponType == PlayerWeaponType.RocketLauncher)
            {
                bulletCon = Instantiate(_rocketPrefab, _bulletPoolParent).GetComponent<BulletCon>();
            }
            else 
            {
               
            }
            bulletCon.OnRelease += ReleaseBullet;
            return bulletCon;
        }

        private void OnGetObj(BulletCon bulletCon, PlayerWeaponType playerWeaponType)
        {
            if (playerWeaponType == PlayerWeaponType.AssaultRifle)
            {
                bulletCon.transform.position = _assaultShotPos.position;
            }
            else if (playerWeaponType == PlayerWeaponType.RocketLauncher) 
            {
                bulletCon.transform.position = _rocketShotPos.position;
            }
            bulletCon.SetVisible(true);
        }

        private void DestroyPoolBullet(BulletCon bulletCon)
        {
            Destroy(bulletCon.gameObject);
        }
    }

    public enum PoolObjType
    {
        PlayerBullet,
    }
}