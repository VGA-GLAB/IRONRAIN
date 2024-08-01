using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Enemy.Control
{
    public class EnemyLifetimeScope : LifetimeScope
    {
        [SerializeField] private Transform _player;
        [SerializeField] private DebugPointP _pointP;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_player != null) builder.RegisterComponent(_player);
            if (_pointP != null) builder.RegisterComponent(_pointP);
        }
    }
}
