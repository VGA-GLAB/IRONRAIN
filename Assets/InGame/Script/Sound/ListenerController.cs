using System;
using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

[RequireComponent(typeof(CriAtomListener))]
public sealed class ListenerController : MonoBehaviour
{
    private void Awake()
    {
        // 各チャンネルにListenerを設定
        var listener = GetComponent<CriAtomListener>();
        
        CriAudioManager.Instance.SE.SetListenerAll(listener);
        CriAudioManager.Instance.Voice.SetListenerAll(listener);
    }
}
