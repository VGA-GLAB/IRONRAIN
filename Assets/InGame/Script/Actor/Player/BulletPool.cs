using UnityEngine;
using UnityEngine.Pool;

public class BulletPool: MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _shotPos;
    private ObjectPool<BulletCon> _bulletPool;

    private Transform _bulletPoolParent;

    private void Start()
    {
        var obj = new GameObject();
        obj.name = "BulletPool";
        _bulletPoolParent = Instantiate(obj).transform;
        _bulletPool = new ObjectPool<BulletCon>(
            createFunc: () => InsObj(),
            actionOnGet: x => OnGetObj(x),
            actionOnDestroy: x => DestroyPoolBullet(x),
            maxSize:30
            );
    }

    public BulletCon GetBullet() 
    {
        return _bulletPool.Get();
    }

    public void ReleaseBullet(BulletCon bulletCon) 
    {
        bulletCon.gameObject.SetActive(false);
        _bulletPool.Release(bulletCon);
    }

    private BulletCon InsObj() 
    {
        var bulletCon = Instantiate(_prefab, _bulletPoolParent).GetComponent<BulletCon>();
        bulletCon.OnRelease += ReleaseBullet;
        return bulletCon;
    }

    private void OnGetObj(BulletCon bulletCon) 
    {
        bulletCon.transform.position = _shotPos.position;
        bulletCon.gameObject.SetActive(true);
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