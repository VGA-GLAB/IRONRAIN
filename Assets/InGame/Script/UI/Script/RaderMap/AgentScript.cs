﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AgentScript : MonoBehaviour
{
    /// <summary>マップに表示するイメージの位置 </summary>
    [NonSerialized] public RectTransform RectTransform;
    /// <summary>表示するレーダーマップ </summary>
    [NonSerialized] public RaderMap RaderMap;
    /// <summary> マップに表示するイメージ</summary>
    [SerializeField] public Image Image;
    /// <summary>ロックオン状態 </summary>
    public bool IsLockon = false;   
}
