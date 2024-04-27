using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMove : PlayerComponentBase
{
    public PlayerMoveModel MoveModel { get; private set; }

    [SerializeReference, SubclassSelector]
    private IPlayerStateModel _playerStateModel;

    private void Awake()
    {
        MoveModel = _playerStateModel as PlayerMoveModel;
    }

    protected override void Start()
    {
        _playerStateModel.SetUp(_playerEnvroment, destroyCancellationToken);
        _playerStateModel.Start();
    }

    protected override void Update()
    {
        _playerStateModel.Update();
    }

    protected override void FixedUpdate()
    {
        _playerStateModel.FixedUpdate();
    }

    public override void Dispose()
    {
        _playerStateModel.Dispose();
    }
}
