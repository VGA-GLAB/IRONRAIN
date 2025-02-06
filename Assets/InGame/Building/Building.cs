using System;
using System.Collections;
using UnityEngine;

// 建物を破壊するサンプルクラス
public class Building : MonoBehaviour
{
    // ToDo : アタッチされた時に同時に回転を設定できるようにしたい
    [SerializeField] private GameObject _buildingPrefab;
    [SerializeField] private GameObject _buildingTower;
    [SerializeField] private float _collisionTimer;
    [SerializeField] private float _fallTime;
    [SerializeField] private float _power;
    
    private Rigidbody _rb;
    

    // タイマーが0になったときのイベント
    private Action _onTimerZero;
    
    private void OnDisable()
    {
        _onTimerZero -= AddForce;
    }

    private void Awake()
    {
        Initialize();
    }
    
    public void StartBuildingAnimation()
    {
        _onTimerZero += AddForce;
        StartCoroutine(TimerCoroutine());
    }

    private void Initialize()
    {
        _rb = _buildingTower.GetComponent<Rigidbody>();
    }

    // ctsを発行する必要もないのでUnityのメソッドを使用
    // これによりUnityが勝手に中断処理を行ってくれる
    private IEnumerator TimerCoroutine()
    {
        while (_collisionTimer > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _collisionTimer -= Time.deltaTime;
        }
        
        Debug.Log("タイマーが0になりました");
        _onTimerZero?.Invoke();
    }

    private void AddForce()
    {
        Debug.Log("力を加えた");
        _rb.AddForce(_buildingTower.transform.right * _power);
    }
}