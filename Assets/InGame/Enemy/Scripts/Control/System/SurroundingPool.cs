using Enemy.DebugUse;
using Enemy.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// スロットの位置を指定する用の列挙型
    /// </summary>
    public enum SlotPlace
    {
        Left,
        MiddleLeft,
        Middle,
        MiddleRight,
        Right,
    }

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
        /// ギズモに描画
        /// </summary>
        public override void DrawOnGizmos()
        {
            Color c = IsUsing ? Color.white : ColorExtensions.ThinWhite;
            GizmosUtils.WireSphere(Point, 0.2f, c); // 大きさは適当
            GizmosUtils.WireCircle(Point, Radius, c);
        }
    }

    /// <summary>
    /// プレイヤーの周囲に敵を配置するスロットを作成し管理する。
    /// プレイヤーがY軸以外で回転すると破綻する可能性がある。
    /// </summary>
    public class SurroundingPool : MonoBehaviour
    {
        // スロット数は仕様で決まっている。
        private const int Capacity = 5;

        [Header("プレイヤーを基準にする")]
        [SerializeField] private Transform _player;
        [Header("生成時の設定")]
        [Tooltip("プレイヤーの位置から前方向のオフセット")]
        [SerializeField] private float _forwardOffset = 1.0f;
        [Tooltip("スロット同士の間隔")]
        [SerializeField] private float _space;
        [Tooltip("スロットの半径")]
        [Min(1.0f)]
        [SerializeField] private float _radius = 1.0f;

        private Slot[] _pool;

        /// <summary>
        /// 空きスロットの数
        /// </summary>
        public int EmptySlotCount { get; private set; }
        /// <summary>
        /// 空きスロットが存在するか
        /// </summary>
        public bool IsExistEmptySlot => EmptySlotCount > 0;

        private void Awake()
        {
            CreatePool();
        }

        private void Update()
        {
            UpdateAllSlotPoint();
        }

        private void CreatePool()
        {
            if (_player == null) return;

            _pool = new Slot[Capacity];
            EmptySlotCount = Capacity;

            foreach ((Vector3 point, int index) value in SlotPoints())
            {
                _pool[value.index] = new Slot(value.point, _radius);
            }
        }

        private void UpdateAllSlotPoint()
        {
            if (_player == null || _pool == null) return;

            foreach ((Vector3 point, int index) value in SlotPoints())
            {
                _pool[value.index].Point = value.point;
            }
        }

        // 各スロットの位置と添え字を返す。
        private IEnumerable<(Vector3, int)> SlotPoints()
        {
            // プレイヤーの向きに準ずるため、プレイヤーの前と右方向を基準に決める。
            Vector3 forward = _player.forward * _forwardOffset;
            Vector3 left = -_player.right * (Capacity - 1) * (_radius + _space / 2);
            Vector3 diameter = _player.right * _radius * 2;
            Vector3 space = _player.right * _space;

            for (int i = 0; i < Capacity; i++)
            {
                // 左から順に位置を返していく。
                yield return (_player.position + forward + left + (diameter + space) * i, i);
            }
        }
        
        /// <summary>
        /// スロットを借りる
        /// </summary>
        public bool TryRent(out Slot slot)
        {
            foreach (Slot s in _pool)
            {
                if (!s.IsUsing)
                {
                    EmptySlotCount--;
                    s.IsUsing = true;
                    slot = s;
                    return true;
                }
            }

            slot = null;
            return false;
        }

        /// <summary>
        /// スロットを借りる。
        /// 既に使われている場合でも借りることが出来る。
        /// </summary>
        public Slot Rent(SlotPlace place)
        {
            return _pool[(int)place];
        }

        /// <summary>
        /// スロットを返却する
        /// </summary>
        public void Return(Slot slot)
        {
            if (slot == null) return;

            slot.IsUsing = false;
            EmptySlotCount++;
        }

        private void OnDrawGizmos()
        {
            if (_pool != null)
            {
                foreach (Slot s in _pool) s.DrawOnGizmos();
            }
        }
    }
}