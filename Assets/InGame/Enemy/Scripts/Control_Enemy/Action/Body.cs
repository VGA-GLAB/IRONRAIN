using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 計算した値を実際にオブジェクトに反映する。
    /// </summary>
    public class Body
    {
        private Transform _transform;
        private Transform _offset;
        private Transform _rotate;
        private Renderer[] _renderers;
        private Collider _damageHitBox;

        public Body(Transform transform, Transform offset, Transform rotate, Renderer[] renderers, 
            Collider damageHitBox)
        {
            _transform = transform;
            _offset = offset;
            _rotate = rotate;
            _renderers = renderers;
            _damageHitBox = damageHitBox;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 TransformPosition => _transform.position;

        /// <summary>
        /// オブジェクト自体を無効化して非表示にする。
        /// 撃破された際はこのメソッドで画面から消す。
        /// </summary>
        public void Disable()
        {
            _transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Rendererのみを表示/非表示にする。
        /// 生成後、撃破されていないが画面から消したい場合に有効。
        /// </summary>
        public void RendererEnable(bool value)
        {
            foreach (Renderer r in _renderers)
            {
                r.enabled = value;
            }
        }

        /// <summary>
        /// Rendererが表示/非表示の状態を返す。
        /// </summary>
        public bool IsRendererEnabled()
        {
            // RendererEnableメソッドで全てのRendererに対して一括で表示/非表示の処理をしているが
            // 一応全てのRendererが表示されているか判定する。
            foreach (Renderer r in _renderers)
            {
                if (!r.enabled) return false;
            }

            return true;
        }

        /// <summary>
        /// ダメージの当たり判定のみを有効/無効化する。
        /// 撃破された後、ダメージの判定を残さないために有効。
        /// </summary>
        public void DamageHitBoxEnable(bool value)
        {
            _damageHitBox.enabled = value;
        }


        /// <summary>
        /// オブジェクトの座標を変更する。
        /// </summary>
        public void Warp(in Vector3 position)
        {
            _transform.position = position;
        }

        /// <summary>
        /// オブジェクトをdeltaTimeぶんだけ移動させる。
        /// </summary>
        public void Move(in Vector3 velocity)
        {
            _transform.position += velocity;
        }

        /// <summary>
        /// 動的オフセットをdeltaTimeぶんだけ移動させる。
        /// </summary>
        public void OffsetMove(in Vector3 velocity)
        {
            _offset.position += velocity;
        }

        /// <summary>
        /// 前方向を変更する。
        /// </summary>
        public void Forward(in Vector3 forward)
        {
            _rotate.forward = forward;
        }
    }
}
