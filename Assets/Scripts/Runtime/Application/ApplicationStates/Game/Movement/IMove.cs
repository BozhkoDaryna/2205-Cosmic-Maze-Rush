using UnityEngine;

namespace Application.Game
{
    public interface IMove
    {
        void SetSpeed(float speed);
        void Move(Vector2 direction);
    }
}