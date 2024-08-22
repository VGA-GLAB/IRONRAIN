using UnityEngine;

namespace Enemy.Funnel
{
    /// <summary>
    /// 攻撃方法
    /// </summary>
    public enum FireMode { Forward, Player }

    // 展開
    [System.Serializable]
    public class ExpandSettings
    {
        [Header("高さ")]
        [SerializeField] private float _height = 8.0f;
        [Header("横方向の位置")]
        [SerializeField] private float _side = 5.0f;
        [Header("前後のオフセット")]
        [SerializeField] private float _offset;

        public float Height => _height;
        public float Side => _side;
        public float Offset => _offset;
    }

    /// <summary>
    /// ファンネルの個体毎のパラメータ。
    /// プランナーが弄る。
    /// </summary>
    public class FunnelParams : MonoBehaviour
    {
        [Header("展開時の設定")]
        [SerializeField] private ExpandSettings _expand;

        [Min(1)]
        [Header("体力の最大値")]
        [SerializeField] private int _maxHp = 100;

        [Range(0.01f, 10.0f)]
        [Header("攻撃間隔")]
        [SerializeField] private float _fireRate = 1.0f;
        [Tooltip("最初の弾を発射するタイミングがズレる。")]
        [SerializeField] private bool _randomFirstShot;

        [Header("攻撃方法")]
        [SerializeField] private FireMode _fireMode;

        [Range(0, 1.0f)]
        [Header("攻撃の精度")]
        [Tooltip("値を大きくすると、プレイヤーの正面に弾が飛んでくる確率が上がる。")]
        [SerializeField] private float _accuracy;

        [Range(5.0f, 20.0f)]
        [Header("ボスを追跡する速さ")]
        [SerializeField] private float _moveSpeed;

        public FireMode FireMode => _fireMode;
        public ExpandSettings Expand => _expand;
        public int MaxHp => _maxHp;
        public float FireRate => _fireRate;
        public float Accuracy => _accuracy;
        public float MoveSpeed => _moveSpeed;
        public Armor Armor { get => Armor.None; }
    }
}
