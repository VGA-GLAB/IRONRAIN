using Enemy.Extensions;
using UnityEngine;

namespace Enemy
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

    // 敵側のAwake時点で呼び出せるように実行順を先にしておく。
    // Perceptionへ渡すためだけにEnemyControllerで参照を受け取るのを解消するためシングルトン。
    [DefaultExecutionOrder(-1)]
    public class AreaCalculator : MonoBehaviour
    {
        private static AreaCalculator Instance;

        [SerializeField] private float _space = 3.0f;
        [SerializeField] private float _radius = 1.0f;
        [SerializeField] private float _playerRadius = 1.0f;
        [SerializeField] private float _bossRadius = 1.5f;
        [SerializeField] private float _upperOffset;

        private static float Space => Instance._space;
        private static float Radius => Instance._radius;
        private static float PlayerRadius => Instance._playerRadius;
        private static float BossRadius => Instance._bossRadius;
        private static float UpperOffset => Instance._upperOffset;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void OnDestroy()
        {
            if (Instance != null) Instance = null;
        }

        /// <summary>
        /// スロットの位置を計算する。
        /// プレイヤーがY軸以外で回転すると破綻する可能性がある。
        /// </summary>
        public static Vector3 SlotPoint(Transform player, SlotPlace place, float forwardOffset)
        {
            int length = EnumExtensions.Length<SlotPlace>();

            // プレイヤーの位置
            Vector3 p = player.position;
            // 中心から左側に向けて並べていく。
            p += player.right * Space * (int)place;
            p += -player.right * (length / 2) * Space;
            // 前方向のオフセットを加算
            p += player.forward * forwardOffset;
            // 上下方向のオフセットを加算
            p += Vector3.up * UpperOffset;
            // 偶数個の場合
            if (length % 2 == 0) p += player.right * Space / 2;

            return p;
        }

        /// <summary>
        /// エリアの位置を計算する。
        /// </summary>
        public static Vector3 AreaPoint(Transform owner)
        {
            // スロットの位置をこのクラスで計算するのでそれに合わせる。
            return owner.position;
        }

        /// <summary>
        /// スロットの位置を計算する。
        /// </summary>
        public static Vector3 SlotPoint(Transform player, SlotSettings settings)
        {
            return SlotPoint(player, settings.Place, settings.ForwardOffset);
        }

        /// <summary>
        /// 任意の位置にスロットを作成して返す。
        /// </summary>
        public static Area CreateSlot(Transform player, SlotPlace place, float forwardOffset)
        {
            Vector3 p = SlotPoint(player, place, forwardOffset);
            return new Area(p, Radius);
        }

        /// <summary>
        /// 任意の位置にスロットを作成して返す。
        /// </summary>
        public static Area CreateSlot(Transform player, SlotSettings settings)
        {
            return CreateSlot(player, settings.Place, settings.ForwardOffset);
        }

        /// <summary>
        /// 敵キャラクターの現在位置を示すエリアを作成して返す。
        /// </summary>
        public static Area CreateArea(Vector3 point)
        {
            return new Area(point, Radius);
        }

        /// <summary>
        /// ボスの現在位置を示すエリアを作成して返す。
        /// </summary>
        public static Area CreateBossArea(Vector3 point)
        {
            return new Area(point, BossRadius);
        }

        /// <summary>
        /// プレイヤーの現在位置を示すエリアを作成して返す。
        /// </summary>
        public static Area CreatePlayerArea(Transform player)
        {
            return new Area(player.position, PlayerRadius);
        }
    }
}
