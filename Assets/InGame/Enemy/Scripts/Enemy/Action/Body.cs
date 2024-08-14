using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 計算した値を実際にオブジェクトに反映する。
    /// </summary>
    public class Body
    {
        private Transform _transform;
        private Transform _offset;
        private Transform _rotate;
        private Transform[] _models;
        private Collider[] _hitBoxes;

        public Body(RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _offset = requiredRef.Offset;
            _rotate = requiredRef.Rotate;
            _models = requiredRef.Models;
            _hitBoxes = requiredRef.HitBoxes;
        }

        public Body(Boss.RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _offset = requiredRef.Offset;
            _rotate = requiredRef.Rotate;
            _models = requiredRef.Models;
            _hitBoxes = requiredRef.HitBoxes;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position => _transform.position;

        /// <summary>
        /// オブジェクト自体を無効化して非表示にする。
        /// 撃破された際はこのメソッドで画面から消す。
        /// </summary>
        public void Disable()
        {
            _transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// 3Dモデルのみを表示/非表示にする。
        /// 生成後、撃破されていないが画面から消したい場合に有効。
        /// </summary>
        public void ModelEnable(bool value)
        {
            foreach (Transform t in _models)
            {
                t.localScale = value ? Vector3.one : Vector3.zero;
            }
        }

        /// <summary>
        /// 3Dモデルが表示/非表示の状態を返す。
        /// </summary>
        public bool IsModelEnabled()
        {
            // ModelEnableメソッドで全ての3Dモデルに対して一括で表示/非表示の処理をしているが
            // 一応全てが表示されているか判定する。
            foreach (Transform t in _models)
            {
                if (t.localScale != Vector3.zero) return true;
            }

            return false;
        }

        /// <summary>
        /// 各種当たり判定のみを有効/無効化する。
        /// 撃破された後、当たり判定を残さないために有効。
        /// </summary>
        public void HitBoxEnable(bool value)
        {
            foreach (Collider c in _hitBoxes)
            {
                c.enabled = value;
            }
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
