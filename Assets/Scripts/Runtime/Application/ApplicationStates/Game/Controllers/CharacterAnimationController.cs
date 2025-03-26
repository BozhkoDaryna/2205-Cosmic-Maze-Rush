using UnityEngine;

namespace Runtime.Game
{
    public class CharacterAnimationController
    {
        private static readonly int Stopped = Animator.StringToHash(IsStopped);
        private static readonly int HorizontalSpeedHashed = Animator.StringToHash(HorizontalSpeed);
        private static readonly int VerticalSpeedHashed = Animator.StringToHash(VerticalSpeed);

        private const string HorizontalSpeed = "HorizontalSpeed";
        private const string VerticalSpeed = "VerticalSpeed";
        private const string IsStopped = "IsStopped";

        private static float _vertical;
        private static float _horizontal;
        
        private Animator _animator;

        public void SetMovementAnimation(Vector2 moveDirection, bool isMoving)
        {
            SetDirections(moveDirection);
            SetIsStopped(isMoving);
            SetSpeed();
        }

        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }

        private static void SetDirections(Vector2 moveDirection)
        {
            (_horizontal, _vertical) = SetDirection(moveDirection.x, moveDirection.y);
        }

        private static (float a, float b) SetDirection(float fromDirection, float toDirection)
        {
            if (fromDirection != 0 && Mathf.Abs(fromDirection) > Mathf.Abs(toDirection))
            {
                return (fromDirection, 0);
            }

            return (0, toDirection);
        }

        private void SetSpeed()
        {
            if (_horizontal == 0 && _vertical == 0)
                return;
            _animator.SetFloat(HorizontalSpeedHashed, _horizontal);
            _animator.SetFloat(VerticalSpeedHashed, _vertical);
        }

        private void SetIsStopped(bool isMoving)
        {
            _animator.SetBool(Stopped, !isMoving);
        }
    }
}