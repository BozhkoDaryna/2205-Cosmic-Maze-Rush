using System.Threading;
using Application.Services;
using Core;
using Core.Factory;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Application.Game
{
    public class PlayerSpawner : BaseController
    {
        private readonly GameObjectFactory _factory;
        private readonly DiContainer _container;

        private PlayerConfig _player;

        public PlayerSpawner(GameObjectFactory factory, DiContainer container)
        {
            _factory = factory;
            _container = container;
        }

        public override async UniTask Run(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (_player == null)
            {
                _player = await _factory.Create<PlayerConfig>(ConstGame.Player);
                _container.InjectGameObject(_player.gameObject);
            }

            _player.gameObject.SetActive(true);
            await base.Run(cancellationToken);
        }

        public override UniTask Stop()
        {
            if (_player != null)
                _player.gameObject.SetActive(false);
            return base.Stop();
        }

        public PlayerConfig GetPlayer()
        {
            return _player;
        }
    }
}