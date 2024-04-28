namespace Enemy.Control
{
    /// <summary>
    /// 敵の種類を判定するための列挙型。
    /// </summary>
    public enum EnemyType
    {
        Dummy,      // 未割当の場合に返る値
        MachineGun, // 銃持ち
        Launcher,   // ランチャー持ち
        Shield,     // シールド持ち
    }

    /// <summary>
    /// 敵の種類を判定する機能を実装するインターフェース。
    /// </summary>
    public interface IEnemyTypeReader
    {
        public EnemyType Type { get; }
    }
}
