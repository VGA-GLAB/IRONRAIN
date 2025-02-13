using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject _buildingTower;
    [SerializeField,Header("倒れるまでの時間")] private float _fallTime;
    [SerializeField,Header("加える力の量")] private float _power;
    [SerializeField,Header("建物が倒れてから消える時間")] private float _destroyTime;
    
    private Rigidbody _rb;

    private void Awake()
    {
        Initialize();
    }
    
    // ctsを発行する必要もないのでUnityのメソッドを使用
    // これによりUnityが勝手に中断処理を行ってくれる
    public void StartBuildingAnimation()
    {
        StartCoroutine(FallBuildingCoroutine());
        Debug.Log("aaa");
    }

    private void Initialize()
    {
        _rb = _buildingTower.GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    private IEnumerator FallBuildingCoroutine()
    {
        Debug.Log("bbb");
        // 物理演算を有効化
        _rb.isKinematic = false;
        // 回転摩擦を低めに設定
        _rb.angularDrag = 0.1f; 

        // 重心を下げる
        _rb.centerOfMass = new Vector3(0, -1f, 0);

        // 経過時間
        float elapsed = 0f;

        while (elapsed < _fallTime)
        {
            // 時間経過に応じてトルクを加える
            float torqueForce = Mathf.Lerp(0, _power, elapsed / _fallTime);
            // 瞬間的な力を加える
            _rb.AddTorque(Vector3.forward * torqueForce, ForceMode.Impulse); 

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject,_destroyTime);
    }
}