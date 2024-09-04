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

        private Transform _assaultPoolParent;
        private Transform _rocketPoolParent;

        private void Start()
        {
            var obj = new GameObject();
            obj.name = "AssaultPoolParent";
            _assaultPoolParent = Instantiate(obj).transform;

            var obj2 = new GameObject();
            obj.name = "RocketPoolParent";
            _rocketPoolParent = Instantiate(obj).transform;

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

        public void ReleaseBullet(BulletCon bulletCon, PlayerWeaponType playerWeaponType)
        {
            bulletCon.SetVisible(false);
            if (playerWeaponType == PlayerWeaponType.AssaultRifle)
            {
                _assaultRiflePool.Release(bulletCon);
            }
            else if (playerWeaponType == PlayerWeaponType.RocketLauncher) 
            {
                _rocketPool.Release(bulletCon);
            }
        }

        private BulletCon InsObj(PlayerWeaponType playerWeaponType)
        {
            BulletCon bulletCon = null;
            if (playerWeaponType == PlayerWeaponType.AssaultRifle)
            {
                bulletCon = Instantiate(_assaultPrefab, _assaultPoolParent).GetComponent<BulletCon>();
            }
            else if (playerWeaponType == PlayerWeaponType.RocketLauncher)
            {
                bulletCon = Instantiate(_rocketPrefab, _rocketPoolParent).GetComponent<BulletCon>();
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