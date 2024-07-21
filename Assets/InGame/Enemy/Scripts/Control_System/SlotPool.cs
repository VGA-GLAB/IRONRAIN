using Enemy.DebugUse;
using Enemy.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 敵を配置する箇所。
    /// </summary>
    public class Slot : CircleArea
    {
        public Slot(Vector3 point, float radius) : base(point, radius)
        {
            IsUsing = false;
        }

        /// <summary>
        /// プーリングで運用する際に使用中かの判定をするフラグ。
        /// </summary>
        public bool IsUsing { get; set; }

        /// <summary>
        /// ギズモに描画。
        /// </summary>
        public override void DrawOnGizmos()
        {
            Color c = IsUsing ? Color.white : ColorExtensions.ThinWhite;
            GizmosUtils.WireSphere(Point, 0.2f, c); // 大きさは適当
            GizmosUtils.WireCircle(Point, Radius, c);
        }

        /// <summary>
        /// ギズモに描画。
        /// 描画する高さを引数で指定できる。
        /// </summary>
        public override void DrawOnGizmos(Transform owner)
        {
            Vector3 p = new Vector3(Point.x, owner.position.y, Point.z);
            Color c = IsUsing ? Color.white : ColorExtensions.ThinWhite;
            GizmosUtils.WireSphere(p, 0.2f, c); // 大きさは適当
            GizmosUtils.WireCircle(p, Radius, c);
        }
    }

    /// <summary>
    /// 敵を配置するスロットを作成し管理する。
    /// プレイヤーがY軸以外で回転すると破綻する可能性がある。
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class SlotPool : MonoBehaviour
    {
        /// <summary>
        /// スロットの位置を指定する用の列挙型
        /// </summary>
        public enum Place
        {
            Left,
            MiddleLeft,
            Middle,
            MiddleRight,
            Right,
        }

        // 列挙型の長さと同じ数だけスロットを生成
        private static int Quantity = EnumExtensions.Length<Place>();

        [Header("プレイヤーの前方向を基準にする")]
        [SerializeField] private Transform _player;
        [Header("生成の設定")]
        [SerializeField] private float _forwardOffset = 6.0f;
        [SerializeField] private float _space = 3.0f;
        [Header("スロットの設定")]
        [Min(1.0f)]
        [SerializeField] private float _radius = 1.0f;

        private Slot[] _pool;

        /// <summary>
        /// 空きスロットの数
        /// </summary>
        public int EmptyCount { get; private set; }
        /// <summary>
        /// 空きスロットが存在するか
        /// </summary>
        public bool IsExistEmpty => EmptyCount > 0;

        private void Start()
        {
            CreatePool();
        }

        private void Update()
        {
            UpdateSlot();
        }

        private void OnDrawGizmos()
        {
            if (_pool != null && _player != null)
            {
                foreach (Slot s in _pool) s.DrawOnGizmos(_player);
            }
        }

        private void CreatePool()
        {
            if (_player == null) return;

            _pool = new Slot[Quantity];

            // 中心点からプレイヤーの距離を半径とする円
            foreach ((Vector3 point, int index) s in SlotPoint())
            {
                _pool[s.index] = new Slot(s.point, _radius);
            }

            EmptyCount = Quantity;
        }

        private void UpdateSlot()
        {
            if (_player == null || _pool == null) return;

            foreach ((Vector3 point, int index) s in SlotPoint())
            {
                _pool[s.index].Point = s.point;
            }
        }

        private IEnumerable<(Vector3, int)> SlotPoint()
        {
            for (int i = 0; i < Quantity; i++)
            {
                // プレイヤーを基準に左右均等に配置する。
                Vector3 p = _player.position + _player.forward * _forwardOffset;
                Vector3 left = -_player.right * _space * i;
                left += _player.right * (Quantity / 2) * _space;
                if (Quantity % 2 == 0) left += -_player.right * _space / 2;

                yield return (p + left, i);
            }
        }

        /// <summary>
        /// 引数の位置に一番近いスロットを借りる。
        /// </summary>
        public bool TryRent(Vector3 p, out Slot slot)
        {
            slot = null;

            if (_pool == null) return false;

            float min = float.MaxValue;
            foreach (Slot s in _pool)
            {
                float d = (s.Point - p).sqrMagnitude;
                if (d < min)
                {
                    min = d;
                    slot = s;
                }
            }

            return true;
        }

        /// <summary>
        /// 指定したスロットを借りる。
        /// </summary>
        public bool TryRent(Place place, out Slot slot)
        {
            if (_pool == null)
            {
                slot = null;
                return false;
            }
            else
            {
                slot = _pool[(int)place];
                return true;
            }
        }

        /// <summary>
        /// スロットを返却する。
        /// </summary>
        public void Return(Slot slot)
        {
            if (slot == null) return;

            slot.IsUsing = false;
            EmptyCount++;
        }
    }
}