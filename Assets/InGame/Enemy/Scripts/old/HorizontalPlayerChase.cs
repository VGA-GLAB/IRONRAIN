using Enemy.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// プレイヤーを追跡し、水平方向に並ぶような移動を行う。
    /// プレイヤーがY軸以外で回転すると破綻する可能性がある。
    /// </summary>
    public class HorizontalPlayerChase : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [Tooltip("追跡する位置をプレイヤーの位置から前方向にずらす")]
        [SerializeField] private float _forwardOffset = 0;
        [Tooltip("移動速度の制限")]
        [SerializeField] private float _velocityLimit = 10.0f;
        [Tooltip("ほぼ等しいを判定する距離の閾値")]
        [SerializeField] private float _stopSqrDistanceThreshold = 0.1f;
        [Tooltip("前後移動の速さ")]
        [SerializeField] private float _forwardMoveSpeed = 5.0f;
        [Tooltip("左右移動の速さ")]
        [SerializeField] private float _rightMoveSpeed = 5.0f;
        [Tooltip("上下移動の速さ")]
        [SerializeField] private float _upMoveSpeed = 5.0f;
        [Tooltip("プレイヤーから左右にどれだけ離れるか")]
        [SerializeField] private float _rightDistance = 2.0f;

        private Transform _transform;

        // xyz軸それぞれ個別に速度を管理
        private Vector3 _forwardVelocity;
        private Vector3 _rightVelocity;
        private Vector3 _upVelocity;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (_player == null) return;

            // プレイヤーと同じ向き
            _transform.forward = _player.forward;

            // 前後、左右、上下、それぞれの加速度をそれぞれの速度に反映
            _forwardVelocity += ForwardAcceleration(_forwardVelocity);
            _rightVelocity += RightAcceleration(_rightVelocity);
            _upVelocity += UpAcceleration(_upVelocity);

            // 速度制限
            Vector3 velocity = _forwardVelocity + _upVelocity + _rightVelocity;
            velocity = Vector3.ClampMagnitude(velocity, _velocityLimit);

            // 座標を更新
            _transform.position += velocity * Time.deltaTime;
        }

        // 前後移動の加速度
        private Vector3 ForwardAcceleration(in Vector3 forwardVelocity)
        {
            // プレイヤーが縦移動して計算がおかしくならないようy軸を無視
            Vector3 position = _transform.position;
            position.y = 0;
            Vector3 playerPosition = _player.position;
            playerPosition.y = 0;

            // 射影するベクトルの原点
            Vector3 origin = playerPosition + _player.forward * _forwardOffset;

            // このキャラクターのy軸を無視した位置を、原点から右向きのベクトルに射影する
            Vector3 playerRight = origin + _player.right;
            Vector3 vp = VectorProjection(position - playerRight, origin - playerRight) + playerRight;

            // 震えるのを防ぐ
            // ほぼ等しい距離まで来ていたら停止するように逆方向の速度ベクトルを返す。
            if ((vp - position).sqrMagnitude < _stopSqrDistanceThreshold)
            {
                return -forwardVelocity;
            }
            else
            {
                // 方向に速さを乗算したものを加速度として返す
                return (vp - position).normalized * _forwardMoveSpeed;
            }
        }

        // 左右移動の加速度
        private Vector3 RightAcceleration(in Vector3 rightVelocity)
        {
            // プレイヤーが縦移動して計算がおかしくならないようy軸を無視
            Vector3 position = _transform.position;
            position.y = 0;
            Vector3 playerPosition = _player.position;
            playerPosition.y = 0;

            // このキャラクターのy軸を無視した位置を、プレイヤーの位置から前向きのベクトルに射影する
            Vector3 playerForward = playerPosition + _player.forward;
            Vector3 vp = VectorProjection(position - playerForward, playerPosition - playerForward) + playerForward;

            // 射影した位置と自身の位置を結ぶ直線上の任意の箇所を目指すように移動する
            Vector3 target = vp + Mathf.Sign(_rightDistance) * (position - vp).normalized * _rightDistance;

            // 震えるのを防ぐ
            // ほぼ等しい距離まで来ていたら停止するように逆方向の速度ベクトルを返す。
            if ((target - position).sqrMagnitude < _stopSqrDistanceThreshold)
            {
                return -rightVelocity;
            }
            else
            {
                // 方向に速さを乗算したものを加速度として返す
                return (target - position).normalized * _rightMoveSpeed;
            }
        }

        // 上下移動の加速度
        private Vector3 UpAcceleration(in Vector3 upVelocity)
        {
            // 震えるのを防ぐ
            // ほぼ等しい距離まで来ていたら停止するように逆方向の速度ベクトルを返す。
            float diff = _transform.position.y - _player.position.y;
            if (diff * diff < _stopSqrDistanceThreshold)
            {
                return -upVelocity;
            }

            // プレイヤーの位置との差に応じて上下移動する加速度を返す
            if (diff < 0) return Vector3.up * _upMoveSpeed;
            else return Vector3.down * _upMoveSpeed;
        }

        // xz平面上でaをbに射影したベクトルを返す。
        private Vector3 VectorProjection(in Vector3 a, in Vector3 b)
        {
            // y軸は無視
            Vector2 aa = new Vector2 (a.x, a.z);
            Vector2 bb = new Vector2(b.x, b.z);

            Vector2 vp = aa.VectorProjection(bb);
            return new Vector3(vp.x, 0, vp.y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
        }
    }
}