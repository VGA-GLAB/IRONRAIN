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
    /// 敵を配置するスロットを作成し管理する。
    /// プレイヤーがY軸以外で回転すると破綻する可能性がある。
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class SlotPool : MonoBehaviour
    {
        [Header("フィールドの中心点")]
        [SerializeField] private Vector3 _center;
        [SerializeField] private int _piece = 12;
        [Header("プレイヤーを基準にする")]
        [SerializeField] private Transform _player;
        [Header("生成時の設定")]
        [Tooltip("プレイヤーの位置から前方向のオフセット")]
        [SerializeField] private float _forwardOffset = 1.0f;
        [Tooltip("スロット同士の間隔")]
        [SerializeField] private float _space = 10.0f;
        [Tooltip("スロットの半径")]
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
            if (_pool != null)
            {
                foreach (Slot s in _pool) s.DrawOnGizmos();
            }

            DrawLane();
        }

        private void CreatePool()
        {
            if (_player == null) return;

            float a = 2 * Mathf.PI / _piece;
            for(int i = 0; i < _piece; i++)
            {
                float sin = Mathf.Sin(a * i);
                float cos = Mathf.Cos(a * i);
            }

            //_pool = new Slot[Capacity];
            //for (int i = 0; i < _pool.Length; i++)
            //{
            //    Vector3 left = Vector3.left * (Capacity - 1) * (_radius + _space / 2);
            //    Vector3 diameter = Vector3.right * _radius * 2;
            //    Vector3 space = Vector3.right * _space;
            //    _pool[i] = new Slot(_origin + left + (diameter + space) * i, _radius);
            //}

            //EmptyCount = Capacity;
        }

        private void UpdateSlot()
        {
            if (_player == null || _pool == null) return;

            foreach (Slot s in _pool)
            {
                Vector3 p = s.Point;
                p.z = _player.position.z + _forwardOffset;

                s.Point = p;
            }
        }

        private IEnumerable<Vector3> SlotPoint()
        {
            yield break;

            // 円状のフィールドでプレイヤーに直線移動で向かっていく。
            // 1レーンにつき1体？
            // 左右移動したときについてこない
            // 複数体がプレイヤーを検知したときにどうなる？

            // 中心点からプレイヤーの距離を半径とする円を描く。
            // 円周上を目指す。
        }
        
        /// <summary>
        /// スロットを借りる。
        /// </summary>
        public bool TryRent(out Slot slot)
        {
            foreach (Slot s in _pool)
            {
                if (!s.IsUsing)
                {
                    EmptyCount--;
                    s.IsUsing = true;
                    slot = s;
                    return true;
                }
            }

            slot = null;
            return false;
        }

        /// <summary>
        /// 既に使われている場合でもスロットを借りる。
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
            EmptyCount++;
        }

        // レーンを描画
        void DrawLane()
        {
            if (_pool == null) return;

            // 描画用なので適当な値
            const float Half = 500;

            foreach (Slot s in _pool)
            {
                Vector3 side = Vector3.right * (_radius * 2 + _space);
                Vector3 a = s.Point - side;
                Vector3 b = s.Point + side;

                GizmosUtils.Line(a + Vector3.forward * Half, a + Vector3.back * Half, ColorExtensions.ThinWhite);
                GizmosUtils.Line(b + Vector3.forward * Half, b + Vector3.back * Half, ColorExtensions.ThinWhite);
            }
        }
    }
}