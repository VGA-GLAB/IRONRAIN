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
        [SerializeField] private int _piece = 36;
        [Header("プレイヤーを基準にする")]
        [SerializeField] private Transform _player;
        [Header("生成時の設定")]
        [Tooltip("プレイヤーの位置から前方向のオフセット")]
        [SerializeField] private float _forwardOffset = 1.0f;
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

            _pool = new Slot[_piece];

            // 中心点からプレイヤーの距離を半径とする円
            foreach ((Vector3 point, int index) s in SlotPoint())
            {
                _pool[s.index] = new Slot(s.point, _radius);
            }

            EmptyCount = _piece;
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
            float a = 2 * Mathf.PI / _piece;
            float r = (_center - _player.position).magnitude - _forwardOffset;
            for (int i = 0; i < _piece; i++)
            {
                float sin = Mathf.Sin(a * i);
                float cos = Mathf.Cos(a * i);
                Vector3 p = _center + new Vector3(cos, 0, sin) * r;

                yield return (p, i);
            }
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

            foreach (Slot s in _pool)
            {
                GizmosUtils.Line(_center, s.Point, Color.white);
            }

            GizmosUtils.WireCircle(_center, (_center - _player.position).magnitude, Color.white);
        }
    }
}