using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Enemy.Control
{
    public class EnemyLifetimeScope : LifetimeScope
    {
        [SerializeField] private Transform _player;
        [SerializeField] private SlotPool _slotPool;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_player != null) builder.RegisterComponent(_player);
            if (_slotPool != null) builder.RegisterComponent(_slotPool);
        }
    }
}
