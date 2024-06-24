using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerTrackingPhaseMove : PlayerComponentBase
{
    public PlayerTrackingPhaseMoveModel MoveModel { get; private set; }

    private void Awake()
    {
        MoveModel = _playerStateModel as PlayerTrackingPhaseMoveModel;
    }
}
