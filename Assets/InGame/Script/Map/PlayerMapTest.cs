using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapTest : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position; // ローカル変数に格納
        position.z += _speed; // ローカル変数に格納した値を上書き
        transform.position = position; 
    }
}
