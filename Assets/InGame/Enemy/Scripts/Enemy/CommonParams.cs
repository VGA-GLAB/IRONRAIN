using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵キャラクターの種類ごとに共通しているパラメータ。
    /// プログラマが設定する。
    /// </summary>
    [CreateAssetMenu(fileName = "CommonParams_", menuName = "Enemy/Params")]
    public class CommonParams : ScriptableObject
    {
        // 視界
        [System.Serializable]
        public class FovSettings
        {
            [Header("中心位置のオフセット")]
            [SerializeField] private Vector3 _offset;

            public Vector3 Offset => _offset;
        }

        // 位置取りを決めるエリア
        [System.Serializable]
        public class AreaSettings
        {
            [Header("キャラクターの範囲の半径")]
            [Tooltip("この範囲内にプレイヤーを侵入させないように動く")]
            [Min(1.0f)]
            [SerializeField] private float _radius = 2.1f;
            [Header("プレイヤーの範囲の半径")]
            [Tooltip("この範囲内にキャラクターは侵入しないように動く")]
            [Min(1.0f)]
            [SerializeField] private float _playerRadius = 2.71f;

            public float Radius => _radius;
            public float PlayerRadius => _playerRadius;
        }

        // 戦闘
        [System.Serializable]
        public class TacticalSettings
        {
            [Header("ダメージ耐性")]
            [SerializeField] private Armor _armor;

            public Armor Armor => _armor;
        }

        [SerializeField] private EnemyType _type;
        [SerializeField] private FovSettings _fov;
        [SerializeField] private AreaSettings _area;
        [SerializeField] private TacticalSettings _tactical;

        public EnemyType Type => _type;
        public FovSettings FOV => _fov;
        public AreaSettings Area => _area;
        public TacticalSettings Tactical => _tactical;
    }
}
