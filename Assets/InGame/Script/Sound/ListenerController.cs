using System;
using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

[RequireComponent(typeof(CriAtomListener))]
public sealed class ListenerController : MonoBehaviour
{
    [SerializeField] private CriAtomListener _robotListener;
    private Transform _robotListenerTransform;
    
    private void Awake()
    {
        _robotListenerTransform = _robotListener.transform;
        
        // 各チャンネルにListenerを設定
        var cameraListener = GetComponent<CriAtomListener>();
        
        CriAudioManager.Instance.SE.SetListenerAll(_robotListener);
        CriAudioManager.Instance.CockpitSE.SetListenerAll(cameraListener);
        CriAudioManager.Instance.Voice.SetListenerAll(cameraListener);
    }

    private void Update()
    {
        _robotListenerTransform.rotation = transform.rotation;
    }
}
