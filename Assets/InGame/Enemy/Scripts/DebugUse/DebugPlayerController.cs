using Enemy.DebugUse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.DebugUse
{
    /// <summary>
    /// デバッグ用のプレイヤー制御。
    /// </summary>
    public class DebugPlayerController : MonoBehaviour
    {
        [Header("操作設定")]
        [SerializeField] private float _moveSpeed = 10.0f;
        [SerializeField] private float _rotationSpeed = 10.0f;
        [Header("カメラ制御を行う")]
        [SerializeField] private Transform _camera;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            // 前後左右の移動
            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) move += _transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= _transform.forward;
            if (Input.GetKey(KeyCode.A)) move -= _transform.right;
            if (Input.GetKey(KeyCode.D)) move += _transform.right;
            // 上下の移動
            if (Input.GetKey(KeyCode.Q)) move += Vector3.up;
            if (Input.GetKey(KeyCode.E)) move += Vector3.down;

            // 左右に回転
            float rot = 0;
            if (Input.GetKey(KeyCode.LeftArrow)) rot--;
            if (Input.GetKey(KeyCode.RightArrow)) rot++;

            // 高速化
            float mag = 1;
            if (Input.GetKey(KeyCode.LeftShift)) mag++;

            // 移動
            Vector3 deltaMove = move.normalized * mag * _moveSpeed * Time.deltaTime;
            _transform.position += deltaMove;

            // 回転
            float deltaRot = rot * mag * _rotationSpeed * Time.deltaTime;
            _transform.Rotate(new Vector3(0, deltaRot, 0));
        }

        // カメラをプレイヤーの正面を捉えるように制御する
        private void LateUpdate()
        {
            if (_camera == null) return;
            
            _camera.position = _transform.position;
            _camera.forward = _transform.forward;
        }

        private void OnDrawGizmos()
        {
            Vector3 f = transform.position + transform.forward * 3;
            GizmosUtils.Line(transform.position, f, Color.red);
        }
    }
}
