using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    // An enumeration type to represent the various
    // states that the PathFinder can be at any given time.
    public enum PathFinderStatus
    {
        NOT_INITIALISED,
        SUCCESS,
        FAILURE,
        RUNNING
    }

    // Class node. 
    // An abstract class that provides the base class
    // for any type of vertex that you may want to 
    // implement for your pathfinding.
    public abstract class Node<T>
    {
        public T Value { get; }

        public Node(T value)
        {
            Value = value;
        }

        // Get the neighbours for this node.
        // Add derived class must implement this method.
        public abstract List<Node<T>> GetNeighbours();
    }

    // Пример реализации узла для поиска пути
    public class CustomNode : Node<Vector3>
    {
        public CustomNode(Vector3 value) : base(value)
        {
        }

        public override List<Node<Vector3>> GetNeighbours()
        {
            var neighbours = new List<Node<Vector3>>();

            // Здесь нужно добавить логику получения соседей, например:
            // neighbours.Add(new CustomNode(Value + Vector3.right));
            // neighbours.Add(new CustomNode(Value + Vector3.left));
            // neighbours.Add(new CustomNode(Value + Vector3.up));
            // neighbours.Add(new CustomNode(Value + Vector3.down));

            return neighbours;
        }
    }

    public abstract class PathFinder<T>
    {
        #region Delegates for Cost Calculation.

        public delegate float CostFunction(T a, T b);

        public CostFunction HeuristicCost { get; set; }
        public CostFunction NodeTraversalCost { get; set; }

        #endregion

        #region PathFinderNode

        // The PathFinderNode class.
        // This class equates to a node on the tree generated
        // by the pathfinder in its search for the most optimal
        // path. Do not confuse this with the Node defined above.
        // This class encapsulates a Node and hold other
        // attributes needed for the pathfinding search.
        public class PathFinderNode : IComparable<PathFinderNode>
        {
            public PathFinderNode Parent { get; set; }
            public Node<T> Location { get; }

            // The various costs.
            public float FCost { get; private set; }
            public float GCost { get; private set; }
            public float HCost { get; }

            public PathFinderNode(Node<T> location,
                PathFinderNode parent,
                float gCost,
                float hCost)
            {
                Location = location;
                Parent = parent;
                HCost = hCost;
                SetGCost(gCost);
            }

            public void SetGCost(float c)
            {
                GCost = c;
                FCost = GCost + HCost;
            }

            public int CompareTo(PathFinderNode other)
            {
                if (other == null)
                    return 1;
                return FCost.CompareTo(other.FCost);
            }
        }

        #endregion

        #region Properties

        // Add a property that holds the current status of the 
        // PathFinder. By default set it to NOT_INITIALISED
        public PathFinderStatus Status { get; private set; } = PathFinderStatus.NOT_INITIALISED;

        public Node<T> Start { get; private set; }
        public Node<T> Goal { get; private set; }

        // The property to access the current node 
        // that the pathfinder is now at.
        public PathFinderNode CurrentNode { get; private set; }

        #endregion

        #region Open and Closed Lists and Associated Functions.

        protected List<PathFinderNode> openList =
            new List<PathFinderNode>();

        protected List<PathFinderNode> closedList =
            new List<PathFinderNode>();

        protected PathFinderNode GetLeastCostNode(
            List<PathFinderNode> myList)
        {
            var best_index = 0;
            var best_priority = myList[0].FCost;
            for (var i = 1; i < myList.Count; i++)
                if (best_priority > myList[i].FCost)
                {
                    best_priority = myList[i].FCost;
                    best_index = i;
                }

            var n = myList[best_index];
            return n;
        }

        protected int IsInList(List<PathFinderNode> myList, T cell)
        {
            for (var i = 0; i < myList.Count; i++)
                if (EqualityComparer<T>.Default.Equals(myList[i].Location.Value, cell))
                    return i;
            return -1;
        }

        #endregion

        #region Delegates for Action Callbacks

        // We set some delegats to handle change to the internal
        // values during the pathfinding process.
        // These callbacks can be used to display visually
        // the changes to the cells and lists.
        public delegate void DelegatePathFinderNode(PathFinderNode node);

        public DelegatePathFinderNode onDestinationFound;

        public delegate void DelegateNoArguments();

        public DelegateNoArguments onStarted;
        public DelegateNoArguments onRunning;
        public DelegateNoArguments onFailure;
        public DelegateNoArguments onSuccess;

        #endregion

        #region Pathfinding Search Related Functions

        // Reset the internal variables for a new search.
        protected void Reset()
        {
            if (Status == PathFinderStatus.RUNNING)
                // Cannot reset as a pathfinding is
                // currently in progress.
                return;

            CurrentNode = null;
            openList.Clear();
            closedList.Clear();

            Status = PathFinderStatus.NOT_INITIALISED;
        }

        // Step until SUCCESS or FAILURE.
        // Take a search step. The user must call this
        // method until the Status returned is SUCCESS or FAILURE.
        public PathFinderStatus Step()
        {
            closedList.Add(CurrentNode);

            if (openList.Count == 0)
            {
                // We have exhaused our search.
                Status = PathFinderStatus.FAILURE;
                onFailure?.Invoke();
                return Status;
            }

            // Get the least cost element from the openList.
            CurrentNode = GetLeastCostNode(openList);

            openList.Remove(CurrentNode);

            // Check if the node contains the goal cell.
            if (EqualityComparer<T>.Default.Equals(
                CurrentNode.Location.Value, Goal.Value))
            {
                Status = PathFinderStatus.SUCCESS;
                onDestinationFound?.Invoke(CurrentNode);
                onSuccess?.Invoke();
                return Status;
            }

            // Find the neignbours.
            var neighbours = CurrentNode.Location.GetNeighbours();

            // Traverse each of these neighbours for 
            // possible expansion of the search.
            foreach (var cell in neighbours)
                AlgorithmSpecificImplementation(cell);

            Status = PathFinderStatus.RUNNING;
            onRunning?.Invoke();
            return Status;
        }

        protected abstract void AlgorithmSpecificImplementation(Node<T> cell);

        public bool Initialise(Node<T> start, Node<T> goal)
        {
            if (Status == PathFinderStatus.RUNNING)
                // Pathfinding is currently in progress.
                return false;

            Reset();

            Start = start;
            Goal = goal;

            var H = HeuristicCost(Start.Value, Goal.Value);

            var root = new PathFinderNode(Start, null, 0.0f, H);

            openList.Add(root);

            CurrentNode = root;

            onStarted?.Invoke();

            Status = PathFinderStatus.RUNNING;

            return true;
        }

        #endregion
    }

    #region Dijkstra's Algorithm

    // A cconcrete implementation of a Dijkstra PathFinder.
    public class DijkstraPathFinder<T> : PathFinder<T>
    {
        protected override void AlgorithmSpecificImplementation(Node<T> cell)
        {
            if (IsInList(closedList, cell.Value) == -1)
            {
                var G = CurrentNode.GCost + NodeTraversalCost(
                    CurrentNode.Location.Value, cell.Value);

                // Heuristic cost for Dijkstra is 0.
                var H = 0.0f;

                var idOList = IsInList(openList, cell.Value);

                if (idOList == -1)
                {
                    var n = new PathFinderNode(cell, CurrentNode, G, H);
                    openList.Add(n);
                }
                else
                {
                    var oldG = openList[idOList].GCost;
                    if (G < oldG)
                    {
                        openList[idOList].Parent = CurrentNode;
                        openList[idOList].SetGCost(G);
                    }
                }
            }
        }
    }

    #endregion

    #region A* Algorithm

    public class AStarPathFinder<T> : PathFinder<T>
    {
        protected override void AlgorithmSpecificImplementation(Node<T> cell)
        {
            if (IsInList(closedList, cell.Value) == -1)
            {
                var G = CurrentNode.GCost + NodeTraversalCost(
                    CurrentNode.Location.Value, cell.Value);
                var H = HeuristicCost(cell.Value, Goal.Value);

                var idOList = IsInList(openList, cell.Value);

                if (idOList == -1)
                {
                    var n = new PathFinderNode(cell, CurrentNode, G, H);
                    openList.Add(n);
                }
                else
                {
                    var oldG = openList[idOList].GCost;
                    if (G < oldG)
                    {
                        openList[idOList].Parent = CurrentNode;
                        openList[idOList].SetGCost(G);
                    }
                }
            }
        }
    }

    #endregion

    #region Greedy Best-First Search

    public class GreedyPathFinder<T> : PathFinder<T>
    {
        protected override void AlgorithmSpecificImplementation(Node<T> cell)
        {
            if (IsInList(closedList, cell.Value) == -1)
            {
                // G cost for Greedy search is 0.
                float G = 0;

                var H = HeuristicCost(cell.Value, Goal.Value);

                var idOList = IsInList(openList, cell.Value);

                if (idOList == -1)
                {
                    var n = new PathFinderNode(cell, CurrentNode, G, H);
                    openList.Add(n);
                }
                else
                {
                    var oldG = openList[idOList].GCost;
                    if (G < oldG)
                    {
                        openList[idOList].Parent = CurrentNode;
                        openList[idOList].SetGCost(G);
                    }
                }
            }
        }
    }

    #endregion
}