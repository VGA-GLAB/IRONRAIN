using UnityEngine;

// OVRCameraRigにアタッチする。
public class ShootTheBullet : MonoBehaviour
{
    [Header("適当な3Dオブジェクト")]
    [SerializeField] GameObject _bullet;

    Transform _muzzle;
    AudioSource _audio;

    void Awake()
    {
        _muzzle = FindMuzzle(transform);
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 右トリガーもしくはデバッグ用でスペースキーで発射。
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    // rootから再帰的に探す。
    Transform FindMuzzle(Transform parent)
    {
        const string MuzzleName = "RightHandAnchor";

        Transform t = parent.Find(MuzzleName);

        if (t != null) return t;
        if (parent.childCount == 0) return null;

        foreach (Transform child in parent)
        {
            t = parent.Find(MuzzleName);
        }

        return FindMuzzle(parent.GetChild(0));
    }

    // 弾を発射。
    void Shoot()
    {
        if (_audio != null) _audio.Play();

        GameObject g;
        if (_bullet == null)
        {
            g = new GameObject("NullBullet");
            g.transform.position = _muzzle.transform.position;
        }
        else
        {
            g = Instantiate(_bullet, _muzzle.position, Quaternion.identity);
        }

        if (!g.TryGetComponent(out Rigidbody rb)) rb = g.AddComponent<Rigidbody>();
        if (!g.TryGetComponent(out Collider col)) col = g.AddComponent<SphereCollider>();

        const float power = 15.0f;

        rb.AddForce(_muzzle.forward * power, ForceMode.Impulse);
    }
}
