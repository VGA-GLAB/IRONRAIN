using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSetting : ScriptableObject
{
    [Header("キーボードの入力を許可するかどうか")]
    public bool IsKeyBoard;
}
