using System.Threading;
using Application.Services.Audio;
using Core;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Application.Game
{
    public class PlayerMovement : BaseController, IMove
    {
        private readonly PlayerSpawner _playerSpawner;
        private readonly IAudioService _audioService;

        private static Vector2 _moveDirection;

        private PlayerConfig _playerConfig;
        private Vector3 _targetPositions;
        private Vector3 _previousPositions;
        private float _moveSpeed;

        protected PlayerMovement(PlayerSpawner playerSpawner, IAudioService audioService)
        {
            _audioService = audioService;
            _playerSpawner = playerSpawner;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            _playerConfig = _playerSpawner.GetPlayer();

            _moveDirection = Vector2.zero;
            SetStartPosition();

            return base.Run(cancellationToken);
        }

        public void SetSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        public void Move(Vector2 direction)
        {
            if (CurrentState == ControllerState.Run)
            {
                direction = NormalizeDirection(direction);
                SetDirection(direction);

                MovePlayer();
            }
        }

        private void SetDirection(Vector2 direction)
        {
            _moveDirection = direction;
            _targetPositions = new Vector3(_playerConfig.transform.position.x, _playerConfig.transform.position.y, 0);
        }

        private void MovePlayer()
        {
            var movement = new Vector3(_moveDirection.x, _moveDirection.y, 0) * _moveSpeed * Time.deltaTime;
            var position = _playerConfig.PlayerTransform.position;
            position += movement;
            _playerConfig.PlayerTransform.position = position;

            if (Vector2.Distance(_previousPositions, position) > 1.5f)
            {
                _previousPositions = position;
                PlayRandomFootSound();
            }
        }

        private void PlayRandomFootSound()
        {
            var randomFootSound = Random.Range(0, 3);
            var footSound = $"{ConstAudio.FootStepSound}_{randomFootSound}";

            _audioService.PlaySound($"{footSound}");
        }

        private void SetStartPosition()
        {
            _targetPositions = new Vector3(_playerConfig.transform.position.x, _playerConfig.transform.position.y, 0);
            _playerConfig.PlayerTransform.position = _targetPositions;
        }

        private static Vector2 NormalizeDirection(Vector2 direction)
        {
            return direction.magnitude > 0 ? direction.normalized : Vector2.zero;
        }
    }
}