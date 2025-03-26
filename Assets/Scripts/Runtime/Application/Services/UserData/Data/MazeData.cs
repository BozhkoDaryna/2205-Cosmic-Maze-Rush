using System;

namespace Application.Services.UserData
{
    [Serializable]
    public class MazeData
    {
        public int _potionsCount;
        public int _numX;
        public int _numY;
        public float _minDistanceToPlayer;
    }
}