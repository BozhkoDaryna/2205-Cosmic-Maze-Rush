using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Runtime.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Application.Game
{
    public class InputReaderController : BaseController, ITickable
    {
        private const int ShiftingSpeed = 2;
        private const int WalkSpeed = 4;
        private const int RunSpeed = 6;

        private readonly PlayerInput _playerInput;
        private readonly IMove _currentMoveBehaviour;
        private readonly CharacterAnimationController _characterAnimationController;

        private Vector2 _moveDirection;
        private bool _isShifting;
        private bool _isRunning;

        private Action<InputAction.CallbackContext> _onMovePerformed;
        private Action<InputAction.CallbackContext> _onMoveCanceled;
        private Action<InputAction.CallbackContext> _onShiftPerformed;
        private Action<InputAction.CallbackContext> _onShiftCanceled;
        private Action<InputAction.CallbackContext> _onRunPerformed;
        private Action<InputAction.CallbackContext> _onRunCanceled;

        public InputReaderController(IMove playerMovement, CharacterAnimationController characterAnimationController)
        {
            _characterAnimationController = characterAnimationController;
            _currentMoveBehaviour = playerMovement;
            _playerInput = new PlayerInput();
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            _playerInput.Enable();
            InitInputs();
            _characterAnimationController.SetMovementAnimation(Vector2.up, false);
            return base.Run(cancellationToken);
        }

        public override UniTask Stop()
        {
            UnSubscribeActions();

            _playerInput.Disable();

            return base.Stop();
        }

        public void Tick()
        {
            if (CurrentState == ControllerState.Run)
            {
                if (_currentMoveBehaviour != null)
                    ExecuteMove();
                if (_isRunning)
                    ChangeSpeed(RunSpeed);
                else if (_isShifting)
                    ChangeSpeed(ShiftingSpeed);
                else
                    ChangeSpeed(WalkSpeed);
            }
        }

        private void ExecuteMove()
        {
            _currentMoveBehaviour.Move(_moveDirection);
        }

        private void ChangeSpeed(float speed)
        {
            _currentMoveBehaviour.SetSpeed(speed);
        }

        private void ChangeIsMoving(bool isMove, Vector2 direction)
        {
            var flooredHorizontal = Mathf.RoundToInt(direction.x);
            var flooredVertical = Mathf.RoundToInt(direction.y);
            direction = new Vector2(flooredHorizontal, flooredVertical);
            _characterAnimationController.SetMovementAnimation(direction, isMove);
            _moveDirection = isMove ? direction : Vector2.zero;
        }

        private void SetShifting(bool shiftingState)
        {
            _isShifting = shiftingState;
            if (shiftingState)
                ChangeSpeed(ShiftingSpeed);
            else if (!_isRunning)
                ChangeSpeed(WalkSpeed);
        }

        private void SetRun(bool runState)
        {
            _isRunning = runState;
            if (runState)
                ChangeSpeed(RunSpeed);
            else if (!_isShifting)
                ChangeSpeed(WalkSpeed);
        }

        private void InitInputs()
        {
            SetActions();
            SubscribeActions();
        }

        private void SubscribeActions()
        {
            _playerInput.Main.Move.performed += _onMovePerformed;
            _playerInput.Main.Move.canceled += _onMoveCanceled;
            _playerInput.Main.Shifting.performed += _onShiftPerformed;
            _playerInput.Main.Shifting.canceled += _onShiftCanceled;
            _playerInput.Main.Run.performed += _onRunPerformed;
            _playerInput.Main.Run.canceled += _onRunCanceled;
        }

        private void UnSubscribeActions()
        {
            _playerInput.Main.Move.performed -= _onMovePerformed;
            _playerInput.Main.Move.canceled -= _onMoveCanceled;
            _playerInput.Main.Shifting.performed -= _onShiftPerformed;
            _playerInput.Main.Shifting.canceled -= _onShiftCanceled;
            _playerInput.Main.Run.performed -= _onRunPerformed;
            _playerInput.Main.Run.canceled -= _onRunCanceled;
        }

        private void SetActions()
        {
            _onMovePerformed = context => ChangeIsMoving(true, context.ReadValue<Vector2>());
            _onMoveCanceled = context => ChangeIsMoving(false, Vector2.zero);
            _onShiftPerformed = context => SetShifting(true);
            _onShiftCanceled = context => SetShifting(false);
            _onRunPerformed = context => SetRun(true);
            _onRunCanceled = context => SetRun(false);
        }
    }
}