using VContainer;
using VContainer.Unity;
using UnityEngine;
using Enemy.Control.Boss;

namespace Enemy.Control
{
    public class EnemyLifetimeScope : LifetimeScope
    {
        [SerializeField] private Transform _player;
        [SerializeField] private SlotPool _slotPool;
        [SerializeField] private BossStage _bossStage;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_player != null) builder.RegisterComponent(_player);
            if (_slotPool != null) builder.RegisterComponent(_slotPool);
            if (_bossStage != null) builder.RegisterComponent(_bossStage);
        }
    }
}
