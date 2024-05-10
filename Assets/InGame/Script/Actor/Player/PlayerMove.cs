using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMove : PlayerComponentBase
{
    public PlayerMoveModel MoveModel { get; private set; }

    private void Awake()
    {
        MoveModel = _playerStateModel as PlayerMoveModel;
    }
}
