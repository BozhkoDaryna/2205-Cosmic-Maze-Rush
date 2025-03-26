using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Application.Spawners
{
    public class GameLevelView : MonoBehaviour
    {
        private readonly List<BaseEntitySpawner> _spawners = new List<BaseEntitySpawner>();

        [SerializeField] private List<Transform> _firePositions; // todo do a random

        private int _levelIndex;

        [Inject]
        private void Construct(FireSpawner fireSpawner)
        {
            fireSpawner.SetSpawnPositions(_firePositions);

            _spawners.Add(fireSpawner);
        }

        public void Spawn(int levelIndex)
        {
            _levelIndex = levelIndex;

            foreach (var spawner in _spawners)
            {
                spawner.SetView(this);
                var setLevel = spawner as ISetLevel;
                setLevel?.SetLevelIndex(_levelIndex);
                spawner.Spawn();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                Spawn(_levelIndex);
        }
    }
}