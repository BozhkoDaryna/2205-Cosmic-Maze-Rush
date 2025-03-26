using Application.Services;
using Core.Factory;
using Cysharp.Threading.Tasks;

namespace Application.Spawners
{
    public class FireSpawner : BaseEntitySpawner
    {
        public FireSpawner(GameObjectFactory factory) : base(factory)
        {
        }

        public override async UniTask Spawn()
        {
            foreach (var spawnPosition in SpawnPositions)
            {
                var fire = await Factory.Create(ConstGame.Fire);
                fire.transform.position = spawnPosition.position;
                fire.transform.SetParent(View.transform);
            }
        }
    }
}