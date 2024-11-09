/// <summary>
/// 操作説明の列挙型
/// </summary>
public enum IndicationUIType
{
    None,
    /// <summary>外側のレバーを押し出す</summary>
    PushOutsideLever,
    /// <summary>外側のレバーを引く</summary>
    PullOutsideLever,
    /// <summary>スティックレバーで攻撃</summary>
    ControllerTrigger,
    /// <summary>スティックレバーで武器変更</summary>
    ControllerWeaponChange,
    /// <summary>スティックレバーを倒す</summary>
    ControllerMove,
    /// <summary>スロットルレバーを押し出す</summary>
    PushThrottle,
    /// <summary>スロットルレバーを引く</summary>
    PullThrottle,
    /// <summary>スロットルレバー背面のボタンを押す</summary>
    ThrottleTrigger,
    /// <summary>トグルスイッチ</summary>
    Toggle,

}