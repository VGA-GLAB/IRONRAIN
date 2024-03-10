/// <summary>
/// ダメージを受ける処理を実装するインターフェース。
/// </summary>
public interface IDamageable
{
    // NOTE:
    // 敵側がパイルバンカーか銃かでダメージを受けるかの判定を行う必要がある。
    // 現状仕様が固まっていないので、第二引数に文字列で渡せるようにしてある。

    /// <summary>
    /// ダメージを受ける。
    /// </summary>
    /// <param name="value">ダメージ量</param>
    /// <param name="weapon">ダメージを与えた武器</param>
    public void Damage(int value, string weapon = "");
}
