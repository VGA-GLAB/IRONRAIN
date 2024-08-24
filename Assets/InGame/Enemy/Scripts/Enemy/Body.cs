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
        private Renderer[] _renderers;
        private Collider[] _hitBoxes;

        public Body(RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _offset = requiredRef.Offset;
            _rotate = requiredRef.Rotate;
            _renderers = requiredRef.Renderers;
            _hitBoxes = requiredRef.HitBoxes;
        }

        public Body(Boss.RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _offset = requiredRef.Offset;
            _rotate = requiredRef.Rotate;
            _renderers = requiredRef.Renderers;
            _hitBoxes = requiredRef.HitBoxes;
        }

        public Body(Funnel.RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _offset = requiredRef.Offset;
            _rotate = requiredRef.Rotate;
            _renderers = requiredRef.Renderers;
            _hitBoxes = requiredRef.HitBoxes;
        }

        public Body(NPC.RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _offset = requiredRef.Offset;
            _rotate = requiredRef.Rotate;
            _renderers = requiredRef.Renderers;
            _hitBoxes = requiredRef.HitBoxes;
        }

        public Vector3 Position => _transform.position;
        public Vector3 Forward => _rotate.forward;
        public Vector3 Right => _rotate.right;
        public Vector3 Up => _rotate.up;

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
        public void RendererEnable(bool value)
        {
            foreach (Renderer r in _renderers)
            {
                r.enabled = value;
            }
        }

        /// <summary>
        /// 3Dモデルが表示/非表示の状態を返す。
        /// </summary>
        public bool IsModelEnabled()
        {
            // RendererEnableメソッドで全ての3Dモデルに対して一括で表示/非表示の処理をしているが
            // 一応全てが表示されているか判定する。
            foreach (Renderer r in _renderers)
            {
                if (r.enabled) return true;
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
        /// 動的オフセットの座標を変更する。
        /// </summary>
        public void OffsetWarp(in Vector3 position)
        {
            _offset.localPosition = position;
        }

        /// <summary>
        /// 前方向を変更する。
        /// </summary>
        public void LookForward(in Vector3 forward)
        {
            _rotate.forward = forward;
        }
    }
}
