using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;
using UniRx;
using Enemy;

namespace IronRain.Player
{
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
            if (QTEModel == null || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.NonTriggerQte)) return;
            var enemyTypeReader = other.GetComponentInParent<EnemyController>();
            if (enemyTypeReader == null) return;
            //盾持ちの敵が入ってきたら
            if (enemyTypeReader.Params.Type == EnemyType.Shield)
            {
                _guid = enemyTypeReader.BlackBoard.ID;
                QTEModel.StartQTE(_guid, QteType.NormalQte).Forget();
            }
        }
    }
}
