using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Enemy.Control
{
    public class EnemyLifetimeScope : LifetimeScope
    {
        [SerializeField] private Transform _player;
        [SerializeField] private SurroundingPool _surroundingPool;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_player != null) builder.RegisterComponent(_player);
            if (_surroundingPool != null) builder.RegisterComponent(_surroundingPool);
        }
    }
}
