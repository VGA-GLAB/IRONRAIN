using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;
using UniRx;
using Enemy.Control;

public class PlayerQTE : PlayerComponentBase
{
    public PlayerQTEModel QTEModel { get; private set; }

    [SerializeField] private PlayerQTEView _qteView;

    private System.Guid _guid;

    private void Awake()
    {
        QTEModel = _playerStateModel as PlayerQTEModel;
    }

    protected override void Start()
    {
        base.Start();
        QTEModel.QTEType.Subscribe(x => _qteView.SetQteStateText(x)).AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(QTEModel == null || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable)
            || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.NonTriggerQte)) return;
        var enemyTypeReader = other.GetComponentsInParent<EnemyController>();
        if (enemyTypeReader.Length == 0) return;
        Debug.Log(other);
        //盾持ちの敵が入ってきたら
        if (enemyTypeReader[0].Params.Type == EnemyType.Shield)
        {
            _guid = enemyTypeReader[0].BlackBoard.ID;
            QTEModel.StartQTE(_guid, QteType.NormalQte).Forget();
        }
    }
}
