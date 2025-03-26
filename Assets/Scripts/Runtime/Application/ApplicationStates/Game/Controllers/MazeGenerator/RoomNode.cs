using System.Collections.Generic;
using PathFinding;
using UnityEngine;

namespace Runtime.MazeGenerator
{
    public class RoomNode : Node<Vector2Int>
    {
        private readonly MazeGenerator _mazeGenerator;

        public RoomNode(Vector2Int value, MazeGenerator mazeGenerator) : base(value)
        {
            _mazeGenerator = mazeGenerator;
        }

        public override List<Node<Vector2Int>> GetNeighbours()
        {
            return _mazeGenerator.GetNeighbours(Value.x, Value.y);
        }
    }
}