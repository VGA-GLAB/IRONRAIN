using System;

[Flags]
public enum PlayerStateType
{
    /// <summary>スラスター移動中</summary>
    Thruster = 1,
    /// <summary>QTEモーション中</summary>
    QTE = 2,
    /// <summary>修復モード</summary>
    RepairMode = 4,
    /// <summary>操作不能</summary>
    Inoperable = 8,
    /// <summary>武器切り替え</summary>
    SwitchingArms = 16,
    /// <summary>攻撃不可能</summary>
    NonAttack = 32,
}
