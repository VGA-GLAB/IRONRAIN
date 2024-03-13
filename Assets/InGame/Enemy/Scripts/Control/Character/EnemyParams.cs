using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 敵キャラクター毎のパラメータを一括で管理する。
    /// 必要に応じて各クラスに注入する。
    /// </summary>
    [CreateAssetMenu(fileName ="EnemyParams_" ,menuName ="Enemy/Params")]
    public class EnemyParams : ScriptableObject
    {
        // 視界
        [System.Serializable]
        public class FovSettings
        {
            [Header("半径")]
            [Min(0)]
            [SerializeField] private float _radius;
            [Header("中心位置のオフセット")]
            [SerializeField] private Vector3 _offset;

            public float Radius => _radius;
            public Vector3 Offset => _offset;
        }

        // 位置取りを決めるエリア
        [System.Serializable]
        public class AreaSettings 
        {
            [Header("キャラクターの範囲の半径")]
            [Tooltip("この範囲内にプレイヤーを侵入させないように動く")]
            [Min(1.0f)]
            [SerializeField] private float _radius = 1.0f;
            [Header("プレイヤーの範囲の半径")]
            [Tooltip("この範囲内にキャラクターは侵入しないように動く")]
            [Min(1.0f)]
            [SerializeField] private float _playerRadius = 1.0f;

            public float Radius => _radius;
            public float PlayerRadius=> _playerRadius;
        }

        // 移動
        [System.Serializable]
        public class MoveSettings
        {
            [Header("プレイヤーに向かう速さ")]
            [Tooltip("登場後、この速さでプレイヤーに追従する")]
            [Min(1.0f)]
            [SerializeField] private float _chaseSpeed = 1.0f;

            public float ChaseSpeed => _chaseSpeed;
        }

        [Header("視界の設定")]
        [SerializeField] FovSettings _fov;
        [Header("侵入不可能な範囲の設定")]
        [SerializeField] AreaSettings _area;
        [Header("移動の設定")]
        [SerializeField] MoveSettings _move;

        public FovSettings FOV => _fov;
        public AreaSettings Area => _area;
        public MoveSettings Move => _move;
    }
}