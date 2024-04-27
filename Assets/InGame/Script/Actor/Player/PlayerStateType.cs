using System;

[Flags]
public enum PlayerStateType
{
    /// <summary>�X���X�^�[�ړ���</summary>
    Thruster = 1,
    /// <summary>QTE���[�V������</summary>
    QTE = 2,
    /// <summary>�C�����[�h</summary>
    RepairMode = 4,
    /// <summary>����s�\</summary>
    Inoperable = 8,
    /// <summary>����؂�ւ�</summary>
    SwitchingArms = 16,
}
