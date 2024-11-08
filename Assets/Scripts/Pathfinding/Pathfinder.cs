using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Pathfinder
{
    public Pathfinder(GameGrid grid, Dictionary<Vector3, GridCell> cells)
    {
        _nodeParents = new Dictionary<Vector3, Vector3>();
        WalkablePositions = new Dictionary<Vector3, bool>();
        Obstacles = new Dictionary<Vector3, int>();
        _grid = grid;
        UpdateWalkableObstacles(cells);
    }

    public IDictionary<Vector3, bool> WalkablePositions;
    public IDictionary<Vector3, int> Obstacles;

    Dictionary<Vector3, Vector3> _nodeParents;
    private GameGrid _grid;

    public void UpdateWalkableObstacles(Dictionary<Vector3, GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (!WalkablePositions.ContainsKey(cell.Key))
            {
                WalkablePositions.Add(cell.Key, true);
            }
            WalkablePositions[cell.Key] = cell.Value.Walkable;

            if (!WalkablePositions.ContainsKey(cell.Key))
            {
                WalkablePositions.Add(cell.Key, true);
            }
            WalkablePositions[cell.Key] = cell.Value.Walkable;

            if (!Obstacles.ContainsKey(cell.Key))
            {
                Obstacles.Add(cell.Key, 1);
            }
            Obstacles[cell.Key] = cell.Value.ObstacleLevel;
        }
    }
    bool CanMove(Vector3 nextPosition)
    {
        return (WalkablePositions.ContainsKey(nextPosition) ? WalkablePositions[nextPosition] : false);
    }
    IList<Vector3> GetWalkableNodes(Vector3 curr)
    {

        IList<Vector3> walkableNodes = new List<Vector3>();

        IList<Vector3> possibleNodes = new List<Vector3>() {
        new Vector3 (curr.x + 1, curr.y, curr.z),
        new Vector3 (curr.x - 1, curr.y, curr.z),
        new Vector3 (curr.x, curr.y, curr.z + 1),
        new Vector3 (curr.x, curr.y, curr.z - 1),
        new Vector3 (curr.x + 1, curr.y, curr.z + 1),
        new Vector3 (curr.x + 1, curr.y, curr.z - 1),
        new Vector3 (curr.x - 1, curr.y, curr.z + 1),
        new Vector3 (curr.x - 1, curr.y, curr.z - 1)
    };

        foreach (Vector3 node in possibleNodes)
        {
            if (CanMove(node))
            {
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }
    int HeuristicCostEstimate(Vector3 node, Vector3 goal, string heuristic)
    {
        switch (heuristic)
        {
            case "euclidean":
                return EuclideanEstimate(node, goal);
            case "manhattan":
                return ManhattanEstimate(node, goal);
        }

        return -1;
    }
    int EuclideanEstimate(Vector3 node, Vector3 goal)
    {
        return (int)Mathf.Sqrt(Mathf.Pow(node.x - goal.x, 2) +
            Mathf.Pow(node.y - goal.y, 2) +
            Mathf.Pow(node.z - goal.z, 2));
    }
    int ManhattanEstimate(Vector3 node, Vector3 goal)
    {
        return (int)(Mathf.Abs(node.x - goal.x) +
            Mathf.Abs(node.y - goal.y) +
            Mathf.Abs(node.z - goal.z));
    }
    private int Weight(Vector3 node)
    {
        if (Obstacles.Keys.Contains(node))
        {
            return Obstacles[node];
        }
        else
        {
            return 1;
        }
    }
    Vector3 FindShortestPathAStar(Vector3 currentPosition, Vector3 goalPosition, string heuristic)
    {
        IEnumerable<Vector3> validNodes = WalkablePositions.Where(x => x.Value == true).Select(x => x.Key);

        IDictionary<Vector3, int> heuristicScore = new Dictionary<Vector3, int>();

        // Represents g(x) or the distance from start to node "x" (Same meaning as in Dijkstra's "distances")
        IDictionary<Vector3, int> distanceFromStart = new Dictionary<Vector3, int>();

        foreach (Vector3 vertex in validNodes)
        {
            heuristicScore.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
            distanceFromStart.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
        }

        heuristicScore[currentPosition] = HeuristicCostEstimate(currentPosition, goalPosition, heuristic);
        distanceFromStart[currentPosition] = 0;

        // The item dequeued from a priority queue will always be the one with the lowest int value
        //    In this case we will input nodes with their calculated distances from the start g(x),
        //    so we will always take the node with the lowest distance from the queue.
        SimplePriorityQueue<Vector3, int> priorityQueue = new SimplePriorityQueue<Vector3, int>();
        priorityQueue.Enqueue(currentPosition, heuristicScore[currentPosition]);

        while (priorityQueue.Count > 0)
        {
            // Get the node with the least distance from the start
            Vector3 curr = priorityQueue.Dequeue();

            // If our current node is the goal then stop
            if (curr == goalPosition)
            {
                return goalPosition;
            }

            IList<Vector3> neighbors = GetWalkableNodes(curr);

            foreach (Vector3 node in neighbors)
            {
                // Get the distance so far, add it to the distance to the neighbor
                int currScore = distanceFromStart[curr] + Weight(node);

                // If our distance to this neighbor is LESS than another calculated shortest path
                //    to this neighbor, set a new node parent and update the scores as our current
                //    best for the path so far.
                if (currScore < distanceFromStart[node])
                {
                    _nodeParents[node] = curr;
                    distanceFromStart[node] = currScore;

                    int hScore = distanceFromStart[node] + HeuristicCostEstimate(node, goalPosition, heuristic);
                    heuristicScore[node] = hScore;

                    // If this node isn't already in the queue, make sure to add it. Since the
                    //    algorithm is always looking for the smallest distance, any existing entry
                    //    would have a higher priority anyway.
                    if (!priorityQueue.Contains(node))
                    {
                        priorityQueue.Enqueue(node, hScore);
                    }
                }
            }
        }
        return currentPosition;
    }

    public IList<Vector3> FindShortestPath(PathfindingType algorithm, Vector3 startPos, Vector3 objective)
    {
        Vector3 startCellId = _grid.CellIdFromPosition(startPos);
        Vector3 goalCellId = _grid.CellIdFromPosition(objective);

        IList<Vector3> path = new List<Vector3>();
        Vector3 goal;

        switch (algorithm)
        {
            case PathfindingType.AStarEuclid:
                goal = FindShortestPathAStar(startCellId, goalCellId, "euclidean");
                break;
            default:
                goal = FindShortestPathAStar(startCellId, goalCellId, "manhattan");
                break;
        }

        if (goal == startCellId || !_nodeParents.ContainsKey(_nodeParents[goal]))
        {
            return null;
        }

        Vector3 curr = goal;
        while (curr != startPos)
        {
            path.Add(curr);
            curr = _nodeParents[curr];
        }
        return path;
    }

    public enum PathfindingType
    {
        AStarEuclid,
        AStarManhattan
    }
}
