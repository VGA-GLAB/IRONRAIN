using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;
using UniRx;
using Enemy;
using System;

namespace IronRain.Player
{
    public class PlayerQTE : PlayerComponentBase
    {
        public PlayerQTEModel QTEModel { get; private set; }
        public bool IsEnterShield => _isEnterShield;

        [SerializeField] private PlayerQTEView _qteView;

        private System.Guid _guid;

        private bool _isEnterShield;

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
            if (QTEModel == null || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable)) return;
            var enemyTypeReader = other.GetComponentInParent<EnemyController>();
            if (enemyTypeReader == null) return;
            //盾持ちの敵が入ってきたら
            if (enemyTypeReader.Params.Type == EnemyType.Shield
                && other.tag == "QteTrigger"
                && !_playerEnvroment.PlayerState.HasFlag(PlayerStateType.NonTriggerQte))
            {
                _guid = enemyTypeReader.BlackBoard.ID;
                QTEModel.StartQTE(_guid, QteType.NormalQte).Forget();
            }
            else if(enemyTypeReader.Params.Type == EnemyType.Shield
                && other.tag == "QteTrigger") 
            {
                _isEnterShield = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var enemyTypeReader = other.GetComponentInParent<EnemyController>();
            if (enemyTypeReader == null) return;
            
            if (enemyTypeReader.Params.Type == EnemyType.Shield
            && other.tag == "QteTrigger")
            {
                _isEnterShield = false;
            }
        }

        [ContextMenu("Qte")]
        public void QteTest() 
        {
            QTEModel.StartQTE(Guid.Empty, QteType.NormalQte).Forget();
        }
    }
}
