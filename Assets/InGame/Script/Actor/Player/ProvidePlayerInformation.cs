using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class ProvidePlayerInformation
{
    public static Subject<Unit> StartQte = new();
    public static Subject<QTEResultType> EndQte = new();
    public static float TimeScale { get; set; }

    ~ProvidePlayerInformation() 
    {
        StartQte.Dispose();
        EndQte.Dispose();
    }
}

public enum QTEResultType 
{
    success,
    failure
}
