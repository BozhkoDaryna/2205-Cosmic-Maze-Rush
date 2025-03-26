using System.Collections.Generic;
using System.Threading;
using Core.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Application.Spawners
{
    public abstract class BaseEntitySpawner
    {
        private protected readonly GameObjectFactory Factory;

        private protected List<Transform> SpawnPositions;
        private protected GameLevelView View;
        private protected CancellationTokenSource CancellationTokenSource;

        protected BaseEntitySpawner(GameObjectFactory factory)
        {
            Factory = factory;
        }

        public abstract UniTask Spawn();

        public void SetView(GameLevelView view)
        {
            View = view;
        }

        public void SetSpawnPositions(List<Transform> spawnPositions)
        {
            SpawnPositions = spawnPositions;
        }
    }
}