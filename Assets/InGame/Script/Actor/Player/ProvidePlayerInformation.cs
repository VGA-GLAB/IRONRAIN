using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class ProvidePlayerInformation
{
    public static ISubject<Guid> StartQte => _startQte;
    private static Subject<Guid> _startQte = new();
    public static ISubject<QteResultData> EndQte => _endQte;
    private static Subject<QteResultData> _endQte = new();
    public static float TimeScale { get => _timeScale; set => _timeScale = value; }
    
    private static float _timeScale = 1;

    ~ProvidePlayerInformation() 
    {
        _startQte.Dispose();
        _endQte.Dispose();
    }
}

public enum QTEResultType 
{
    Success,
    Failure
}
