using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTest : MonoBehaviour
{
    [SerializeField, Tooltip("移動させる座標")] private Transform _destinationTransform;
    //[SerializeField, Tooltip("移動先の座標")]　private Transform _moveTransform;
    [SerializeField, Tooltip("スクロール速度")] private float _scrollSpeed = 1f;

    [SerializeField, Tooltip("対になるBackGround")]
    private GameObject _pairBackGround;

    [SerializeField, Tooltip("Z座標の差分")] private float _zDifference = 1930f;

    private void FixedUpdate()
    {
        if (this.transform.position.z <= _destinationTransform.position.z)
        {
            this.transform.position = new Vector3(0, 0, _pairBackGround.transform.position.z + _zDifference);
        }
        this.transform.Translate(0, 0, -_scrollSpeed);
    }
}