using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class QTESetEnemyIDSequence : ISequence
    {
        [OpenScriptButton(typeof(QTESetEnemyIDSequence))]
        [Description("QTEを始める前に呼ぶ処理")]
        [Header("盾持ちとQTEを行うSequence")]
        private int _sequenceId;

        private EnemyManager _enemyManager;
        private PlayerQTEModel _playerQteModel;
        
        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
            _playerQteModel = data.PlayerController.SeachState<PlayerQTE>().QTEModel;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            RegisterShieldId();
            return UniTask.CompletedTask;
        }

        /// <summary>QTEにShieldの登録を行う</summary>
        private void RegisterShieldId()
        {
            // Enemyを取得する
            var targetEnemies = new List<EnemyController>();
            if (!_enemyManager.TryGetEnemies(_sequenceId, targetEnemies))
            {
                Debug.LogError("敵を取得できませんでした");
                return;
            }

            // ShieldEnemyを探す
            EnemyController shieldEnemy = null;

            foreach (var enemy in targetEnemies)
            {
                if (enemy.TryGetComponent(out ShieldEquipment shieldEquipment))
                {
                    shieldEnemy = enemy;
                    break;
                }
            }

            if (shieldEnemy == null)
            {
                Debug.LogError("このSequenceには、Shieldがいませんでした。");
                return;
            }
            
            // 敵を登録する
            _playerQteModel.SetEnemyId(shieldEnemy.BlackBoard.ID);
        }

        public void Skip()
        {
            RegisterShieldId();
        }
    }
}
