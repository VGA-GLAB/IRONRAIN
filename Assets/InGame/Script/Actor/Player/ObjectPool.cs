using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Assertions;

namespace IronRain.Player
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject _assaultPrefab;
        [SerializeField] private GameObject _rocketPrefab;
        [SerializeField] private GameObject _assaultRifleImpactEffect;
        [SerializeField] private Transform _assaultShotPos;
        [SerializeField] private Transform _rocketShotPos;

        private ObjectPool<BulletCon> _assaultRiflePool;
        private ObjectPool<BulletCon> _rocketPool;
        private ObjectPool<Effect> _assaultRifleImpactEffectPool;
        private Transform _assaultPoolParent;
        private Transform _rocketPoolParent;
        private Transform _effectPoolParent;

        private void Start()
        {
            var obj = new GameObject();
            obj.name = "AssaultPoolParent";
            _assaultPoolParent = Instantiate(obj).transform;

            var obj2 = new GameObject();
            obj.name = "RocketPoolParent";
            _rocketPoolParent = Instantiate(obj).transform;

            var obj3 = new GameObject();
            obj3.name = "EffectPoolParent";
            _effectPoolParent = Instantiate(obj3).transform;

            _assaultRiflePool = new ObjectPool<BulletCon>(
                createFunc: () => InsBulletObj(PoolObjType.AssaultRifle),
                actionOnGet: x => OnGetBulletObj(x, PoolObjType.AssaultRifle),
                actionOnDestroy: x => DestroyPoolBullet(x),
                maxSize: 30
                );
            _rocketPool = new ObjectPool<BulletCon>(
                createFunc: () => InsBulletObj(PoolObjType.RocketLauncher),
                actionOnGet: x => OnGetBulletObj(x, PoolObjType.RocketLauncher),
                actionOnDestroy: x => DestroyPoolBullet(x),
                maxSize: 10
                );

            _assaultRifleImpactEffectPool = new ObjectPool<Effect>(
                createFunc: () => InsEffectObj(PoolObjType.AssaultRifleImpactEffect),
                actionOnGet: x => OnGetEffectObj(x, PoolObjType.AssaultRifleImpactEffect),
                maxSize: 30
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

        public Effect GetEffect(PoolObjType poolObj) 
        {
            if (poolObj == PoolObjType.AssaultRifleImpactEffect)
            {
                return _assaultRifleImpactEffectPool.Get();
            }
            else 
            {
                return null;
            }
        }

        public void ReleaseBullet(BulletCon bulletCon)
        {
            if (bulletCon.WeaponType == PlayerWeaponType.AssaultRifle)
            {
                _assaultRiflePool.Release(bulletCon);
                bulletCon.SetVisible(false);
            }
            else if (bulletCon.WeaponType == PlayerWeaponType.RocketLauncher) 
            {
                _rocketPool.Release(bulletCon);
                bulletCon.SetVisible(false);
            }
        }

        public void ReleaseEffect(Effect particleEffect) 
        {
            particleEffect.gameObject.SetActive(false);
            _assaultRifleImpactEffectPool.Release(particleEffect);
        }

        private BulletCon InsBulletObj(PoolObjType playerWeaponType)
        {
            BulletCon bulletCon = null;
            if (playerWeaponType == PoolObjType.AssaultRifle)
            {
                bulletCon = Instantiate(_assaultPrefab, _assaultPoolParent).GetComponent<BulletCon>();
            }
            else if (playerWeaponType == PoolObjType.RocketLauncher)
            {
                bulletCon = Instantiate(_rocketPrefab, _rocketPoolParent).GetComponent<BulletCon>();
            }
            else 
            {
               
            }
            return bulletCon;
        }

        public Effect InsEffectObj(PoolObjType poolObj)  
        {
            Effect particleEffect = null;
            if (poolObj == PoolObjType.AssaultRifleImpactEffect) 
            {
                particleEffect = Instantiate(_assaultRifleImpactEffect, _effectPoolParent).GetComponent<Effect>();
            }
            return particleEffect;
        }

        private void OnGetBulletObj(BulletCon bulletCon, PoolObjType playerWeaponType)
        {
            if (playerWeaponType == PoolObjType.AssaultRifle)
            {
                bulletCon.transform.position = _assaultShotPos.position;
            }
            else if (playerWeaponType == PoolObjType.RocketLauncher) 
            {
                bulletCon.transform.position = _rocketShotPos.position;
            }
            bulletCon.SetVisible(true);
        }


        private void OnGetEffectObj(Effect effect, PoolObjType playerWeaponType)
        {
            effect.gameObject.SetActive(true);
        }

        private void DestroyPoolBullet(BulletCon bulletCon)
        {
            Destroy(bulletCon.gameObject);
        }
    }

    public enum PoolObjType
    {
        AssaultRifle,
        RocketLauncher,
        AssaultRifleImpactEffect,
    }
}