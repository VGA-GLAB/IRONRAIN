using System;

/// <summary>
/// Enemy.BlackBoard や Enemy.Funnel.BlackBoard をラップするクラス
/// </summary>
public class BlackBoardWrapper
{
    private object _blackBoard; // 元の BlackBoard インスタンス
    private Func<int> _getHp; // 現在のHPを取得するためのデリゲート
    private Func<bool> _getIsAlive; // 現在の生存状態を取得するためのデリゲート

    /// <summary>
    /// 現在の HP を取得（BlackBoard の種類に関わらず統一的にアクセス可能）
    /// </summary>
    public int Hp => _getHp != null ? _getHp() : 0;
    
    /// <summary>
    /// 敵が生存しているかどうかを取得
    /// </summary>
    public bool IsAlive => _getIsAlive != null && _getIsAlive();

    /// <summary>
    /// 渡された BlackBoard インスタンスに応じて適切なデリゲートを設定する
    /// </summary>
    public BlackBoardWrapper(object blackBoard)
    {
        _blackBoard = blackBoard;
        
        if (blackBoard is Enemy.BlackBoard enemyBB)
        {
            // 通常エネミーの場合
            _getHp = () => enemyBB.Hp;
            _getIsAlive = () => enemyBB.IsAlive;
        }
        else if (blackBoard is Enemy.Funnel.BlackBoard funnelBB)
        {
            // ファンネルの場合
            _getHp = () => funnelBB.Hp;
            _getIsAlive = () => funnelBB.IsAlive;
        }
        else
        {
            // デフォルト値
            _getHp = () => 0;
            _getIsAlive = () => false;
        }
    }
}
