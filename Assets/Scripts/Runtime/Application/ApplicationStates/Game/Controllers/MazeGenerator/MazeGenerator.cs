using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Application.EndLevelVariations;
using Application.Services;
using Application.Services.UserData;
using Core;
using Core.UI;
using Cysharp.Threading.Tasks;
using PathFinding;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Runtime.MazeGenerator
{
    public class MazeGenerator
    {
        private readonly GameObject _arrowPrefab;
        private readonly GameObject _roomPrefab;
        private readonly TriggerDetector _doorPrefab;

        private readonly Stack<Room> _stack = new Stack<Room>();
        private readonly List<GameObject> _roomList = new List<GameObject>();
        private readonly List<FirePotion> _firePotions = new List<FirePotion>();
        private readonly FirePotion _firePotionPrefab;
        private readonly float _potionPower;

        private Room[,] _rooms;

        private float _roomWidth;
        private CancellationToken _cancellationToken;
        private MazeData _mazeData;
        private float _roomHeight;
        private bool _generating;
        private FirePotion _newFirePotion;
        private HintPopup _hintPopup;
        private TriggerDetector _spawnedDoor;
        private GameObject _player;

        public event Action CreateHintPopupEvent;
        public event Action<float> UsedPotionEvent;
        public event Action<GameResult> LoadNextLevelEvent;

        public MazeGenerator(GameObject roomPrefab, TriggerDetector doorPrefab, FirePotion firePotionPrefab,
            GameObject arrowPrefab, float potionPower, IAssetProvider assetProvider
        )
        {
            _assetProvider = assetProvider;
            _potionPower = potionPower;
            _firePotionPrefab = firePotionPrefab;
            _roomPrefab = roomPrefab;
            _doorPrefab = doorPrefab;
            _arrowPrefab = arrowPrefab;
        }

        public void LoadFollowingLevel(MazeData mazeData)
        {
            _mazeData = mazeData;
            LoadLevel(_cancellationToken);
        }

        public void Config(GameObject player, HintPopup hitPopup)
        {
            _player = player;
            _hintPopup = hitPopup;
        }

        public void Reset()
        {
            if (_rooms != null)
                foreach (var room in _rooms)
                    if (room.gameObject != null)
                        Object.Destroy(room.gameObject);

            if (_firePotions != null && _firePotions.Count > 0)
            {
                foreach (var firePotion in _firePotions)
                    if (firePotion != null && firePotion.gameObject != null)
                    {
                        firePotion.DetectedEvent -= UsedPotion;
                        Object.Destroy(firePotion.gameObject);
                    }

                _firePotions.Clear();
            }

            if (_floor != null)
                Object.Destroy(_floor);

            if (_spawnedDoor != null)
            {
                _spawnedDoor.DetectedEvent -= LoadNextLevel;
                Object.Destroy(_spawnedDoor.gameObject);
            }

            _stack?.Clear();
        }

        private void LoadLevel(CancellationToken cancellationToken)
        {
            GetRoomSize();
            _rooms = new Room[_mazeData._numX, _mazeData._numY];
            for (var i = 0; i < _mazeData._numX; ++i)
            for (var j = 0; j < _mazeData._numY; ++j)
            {
                var room = Object.Instantiate(_roomPrefab, new Vector3(i * _roomWidth, j * _roomHeight, 0.0f),
                    Quaternion.identity);
                _roomList.Add(room);

                room.name = "Room_" + i + "_" + j;
                _rooms[i, j] = room.GetComponent<Room>();
                _rooms[i, j].Index = new Vector2Int(i, j);
            }

            CreateWithTime(cancellationToken).Forget();
        }

        private async UniTask CreateFloor(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            _floor = await _assetProvider.Instantiate(ConstGame.Floor);

            var centerX = (_mazeData._numX - 1) * _roomWidth / 2f;
            var centerY = (_mazeData._numY - 1) * _roomHeight / 2f;
            var centerPosition = new Vector3(centerX, centerY, 0f);

            _floor.transform.position = centerPosition;
            var floorSpriteRenderer = _floor.GetComponent<SpriteRenderer>().size;
            floorSpriteRenderer.x = _mazeData._numX * 11;
            floorSpriteRenderer.y = _mazeData._numY * 11;
            _floor.GetComponent<SpriteRenderer>().size = floorSpriteRenderer;
        }

        private async UniTask CreateWithTime(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await UniTask.WaitForFixedUpdate();
            if (!_generating)
                CreateMaze();
            FindPath();
            CreateFloor(cancellationToken).Forget();
        }

        private Vector2Int GetDirection(Vector2Int from, Vector2Int to)
        {
            return new Vector2Int(to.x - from.x, to.y - from.y);
        }

        private float GetDirectionAngle(Vector2Int direction, out string directionName)
        {
            if (direction.x > 0)
            {
                directionName = "right";
                return 0;
            }

            if (direction.x < 0)
            {
                directionName = "left";
                return 180;
            }

            if (direction.y > 0)
            {
                directionName = "up";
                return 90;
            }

            if (direction.y < 0)
            {
                directionName = "down";
                return -90f;
            }

            directionName = "Nothing";
            return 0;
        }

        private void CreateArrow(float angle, string name)
        {
            if (_arrowPrefab == null)
                return;

            _hintPopup.AddArrow(_arrowPrefab, angle, name);
        }

        private void OnSuccessPathFinding()
        {
            CreateHintPopupEvent?.Invoke();

            var node = _pathFinder.CurrentNode;
            var pathPoints = new List<Vector2Int>();
            while (node != null)
            {
                pathPoints.Add(node.Location.Value);
                node = node.Parent;
            }

            pathPoints.Reverse();
            if (pathPoints.Count < 2)
                return;
            var initialDirection = GetDirection(pathPoints[0], pathPoints[1]);
            CreateArrow(GetDirectionAngle(initialDirection, out var directionName), directionName);
            for (var i = 1; i < pathPoints.Count - 1; i++)
            {
                var prevDirection = GetDirection(pathPoints[i - 1], pathPoints[i]);
                var nextDirection = GetDirection(pathPoints[i], pathPoints[i + 1]);
                if (prevDirection != nextDirection)
                    CreateArrow(GetDirectionAngle(nextDirection, out directionName), directionName);
            }
        }

        private void SetPlayerPositions(Room startRoom)
        {
            _startRoom = startRoom;
            var playerPosition = startRoom.transform.position;
            if (_player is not null)
                _player.transform.position = playerPosition;
        }

        private void RemoveRoomWall(int x, int y, Room.Directions dir)
        {
            if (dir != Room.Directions.NONE)
                _rooms[x, y].SetDirFlag(dir, false);
            var opp = Room.Directions.NONE;
            switch (dir)
            {
                case Room.Directions.TOP:
                    if (y < _mazeData._numY - 1)
                    {
                        opp = Room.Directions.BOTTOM;
                        ++y;
                    }

                    break;
                case Room.Directions.RIGHT:
                    if (x < _mazeData._numX - 1)
                    {
                        opp = Room.Directions.LEFT;
                        ++x;
                    }

                    break;
                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        opp = Room.Directions.TOP;
                        --y;
                    }

                    break;
                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        opp = Room.Directions.RIGHT;
                        --x;
                    }

                    break;
            }

            if (opp != Room.Directions.NONE)
                _rooms[x, y].SetDirFlag(opp, false);
        }

        private void GetRoomSize()
        {
            var spriteRenderers = _roomPrefab.GetComponentsInChildren<SpriteRenderer>();
            var minBounds = Vector3.positiveInfinity;
            var maxBounds = Vector3.negativeInfinity;
            foreach (var ren in spriteRenderers)
            {
                minBounds = Vector3.Min(minBounds, ren.bounds.min);
                maxBounds = Vector3.Max(maxBounds, ren.bounds.max);
            }

            _roomWidth = maxBounds.x - minBounds.x;
            _roomHeight = maxBounds.y - minBounds.y;
        }

        private Room GetFurthestExitRoom()
        {
            var playerPosition = _startRoom.Index;
            var edgeRooms = new List<Room>();

            for (var x = 0; x < _mazeData._numX; x++)
            {
                if (_rooms[x, 0] != null)
                    edgeRooms.Add(_rooms[x, 0]);
                if (_rooms[x, _mazeData._numY - 1] != null)
                    edgeRooms.Add(_rooms[x, _mazeData._numY - 1]);
            }

            for (var y = 1; y < _mazeData._numY; y++)
            {
                if (_rooms[0, y] != null)
                    edgeRooms.Add(_rooms[0, y]);
                if (_rooms[_mazeData._numX - 1, y] != null)
                    edgeRooms.Add(_rooms[_mazeData._numX - 1, y]);
            }

            edgeRooms.RemoveAll(room =>
                Vector2Int.Distance(playerPosition, room.Index) <= _mazeData._minDistanceToPlayer);

            edgeRooms.Sort((a, b) =>
                Vector2Int.Distance(a.Index, playerPosition).CompareTo(Vector2Int.Distance(b.Index, playerPosition)));

            if (edgeRooms.LastOrDefault() == null)
            {
                Debug.LogError("No valid exit room found." + _startRoom);
                return null;
            }

            return edgeRooms.LastOrDefault();
        }

        private Room RandomizeStartRoom(bool isRoomForPotion)
        {
            var startX = Random.Range(0, _mazeData._numX);
            var startY = Random.Range(0, _mazeData._numY);

            if (isRoomForPotion)
                if (_rooms[startX, startY] == _exitRoom ||
                    Vector2.Distance(_rooms[startX, startY].Index, _startRoom.Index) <= _mazeData._minDistanceToPlayer)
                    return RandomizeStartRoom(true);

            return _rooms[startX, startY];
        }

        private List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(int cx, int cy)
        {
            var neighbours = new List<Tuple<Room.Directions, Room>>();
            foreach (Room.Directions dir in Enum.GetValues(typeof(Room.Directions)))
            {
                var x = cx;
                var y = cy;
                switch (dir)
                {
                    case Room.Directions.TOP:
                        if (y < _mazeData._numY - 1)
                        {
                            ++y;
                            if (!_rooms[x, y].visited)
                                neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.TOP, _rooms[x, y]));
                        }

                        break;
                    case Room.Directions.RIGHT:
                        if (x < _mazeData._numX - 1)
                        {
                            ++x;
                            if (!_rooms[x, y].visited)
                                neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.RIGHT, _rooms[x, y]));
                        }

                        break;
                    case Room.Directions.BOTTOM:
                        if (y > 0)
                        {
                            --y;
                            if (!_rooms[x, y].visited)
                                neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.BOTTOM, _rooms[x, y]));
                        }

                        break;
                    case Room.Directions.LEFT:
                        if (x > 0)
                        {
                            --x;
                            if (!_rooms[x, y].visited)
                                neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.LEFT, _rooms[x, y]));
                        }

                        break;
                }
            }

            return neighbours;
        }

        private bool GenerateStep()
        {
            if (_stack.Count == 0)
                return true;
            var r = _stack.Peek();
            var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);
            if (neighbours.Count > 0)
            {
                var randomIndex = Random.Range(0, neighbours.Count);
                var randomIndex2 = Random.Range(0, neighbours.Count);
                var neighbour = neighbours[randomIndex].Item2;
                var neighbour2 = neighbours[randomIndex2].Item2;

                RemoveRoomWall(r.Index.x, r.Index.y, neighbours[randomIndex].Item1);
                RemoveRoomWall(r.Index.x, r.Index.y, neighbours[randomIndex2].Item1);
                neighbour.visited = true;
                neighbour2.visited = true;
                _stack.Push(neighbour);
                _stack.Push(neighbour2);
            }
            else
            {
                _stack.Pop();
            }

            return false;
        }

        private void CreateMaze()
        {
            _generating = true;
            var startRoom = RandomizeStartRoom(false);
            startRoom.visited = true;
            _stack.Push(startRoom);

            while (!GenerateStep())
            {
            }

            SetPlayerPositions(startRoom);
            _spawnedDoor = Object.Instantiate(_doorPrefab, Vector3.zero, Quaternion.identity);
            _exitRoom = GetFurthestExitRoom();
            _spawnedDoor.transform.position = _exitRoom.transform.position;
            _spawnedDoor.DetectedEvent += LoadNextLevel;
            _spawnedDoor.transform.SetParent(_exitRoom.transform);
            _generating = false;

            for (var i = 0; i < _mazeData._potionsCount; i++)
            {
                _newFirePotion = Object.Instantiate(_firePotionPrefab, Vector3.zero, Quaternion.identity);
                _newFirePotion.transform.position = RandomizeStartRoom(true).transform.position;
                _newFirePotion.DetectedEvent += UsedPotion;
                _firePotions.Add(_newFirePotion);
            }
        }

        private void UsedPotion()
        {
            UsedPotionEvent?.Invoke(_potionPower);
        }

        private void LoadNextLevel()
        {
            LoadNextLevelEvent?.Invoke(GameResult.Victory);
        }

        #region PathFinding

        public List<Node<Vector2Int>> GetNeighbours(int xx, int yy)
        {
            var neighbours = new List<Node<Vector2Int>>();
            foreach (Room.Directions dir in Enum.GetValues(typeof(Room.Directions)))
            {
                var x = xx;
                var y = yy;
                switch (dir)
                {
                    case Room.Directions.TOP:
                        if (y < _mazeData._numY - 1)
                            if (!_rooms[x, y].GetDirFlag(dir))
                            {
                                ++y;
                                neighbours.Add(new RoomNode(new Vector2Int(x, y), this));
                            }

                        break;
                    case Room.Directions.RIGHT:
                        if (x < _mazeData._numX - 1)
                            if (!_rooms[x, y].GetDirFlag(dir))
                            {
                                ++x;
                                neighbours.Add(new RoomNode(new Vector2Int(x, y), this));
                            }

                        break;
                    case Room.Directions.BOTTOM:
                        if (y > 0)
                            if (!_rooms[x, y].GetDirFlag(dir))
                            {
                                --y;
                                neighbours.Add(new RoomNode(new Vector2Int(x, y), this));
                            }

                        break;
                    case Room.Directions.LEFT:
                        if (x > 0)
                            if (!_rooms[x, y].GetDirFlag(dir))
                            {
                                --x;
                                neighbours.Add(new RoomNode(new Vector2Int(x, y), this));
                            }

                        break;
                }
            }

            return neighbours;
        }

        private static float ManhattanCost(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private static float CostBetweenTwoCells(Vector2Int a, Vector2Int b)
        {
            return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }

        private readonly AStarPathFinder<Vector2Int> _pathFinder = new AStarPathFinder<Vector2Int>();
        private Room _exitRoom;
        private Room _startRoom;
        private readonly IAssetProvider _assetProvider;
        private GameObject _floor;

        private void FindPath()
        {
            _pathFinder.HeuristicCost = ManhattanCost;
            _pathFinder.NodeTraversalCost = CostBetweenTwoCells;
            _pathFinder.Initialise(new RoomNode(_startRoom.Index, this), new RoomNode(_exitRoom.Index, this));
            FindPathStep();
        }

        private void FindPathStep()
        {
            while (_pathFinder.Status == PathFinderStatus.RUNNING)
                _pathFinder.Step();
            if (_pathFinder.Status == PathFinderStatus.SUCCESS)
                OnSuccessPathFinding();
        }

        #endregion
    }
}