using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Pathfinder //Algorithm that returns a list of nodes/vectors to make a path between two given locations
{
    public Pathfinder(GameGrid grid, Dictionary<Vector2, GridCell> cells)
    {
        _nodeParents = new Dictionary<Vector2, Vector2>();
        WalkablePositions = new Dictionary<Vector2, bool>();
        Obstacles = new Dictionary<Vector2, int>();
        _grid = grid;
        UpdateWalkableObstacles(cells);
    }

    public IDictionary<Vector2, bool> WalkablePositions;
    public IDictionary<Vector2, int> Obstacles;

    IDictionary<Vector2, Vector2> _nodeParents;
    private GameGrid _grid;

    public void UpdateWalkableObstacles(Dictionary<Vector2, GridCell> cells) //on this method each cell from the dictionary passed is registered in the walkablePosition dictionary and the obstacles dictionary.
                                                                               //Consider separating then in two different functions later
    {
        foreach (var cell in cells)
        {
            if (!WalkablePositions.ContainsKey(cell.Key)) //if the walkablePosition dictionary does not contain the cell requested already, then add it to the dictionary with a value of true (walkable cell)
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
    public void UpdateCellAfterbuildingPlaced(Vector2 pos, bool bWalkable, int obstacleLevel) //this function gets an element in the dictionary walkablePositions using the vector2 aka key, and updates the value (its walkable or not).
                                                                                              //Consider adding another line where updates the obstacles dictionary with the desire level of obstacle
    {
        WalkablePositions[pos] = bWalkable;
        Obstacles[pos] = obstacleLevel;
        Debug.Log($"Updating cell {pos}, new walkable value is {bWalkable} and obstacle level is {obstacleLevel}");
    }

    bool CanMove(Vector2 nextPosition) //checks if the next position is within walkablePosition dictionary, if it is retures can move else returns cannot move
    {
        return (WalkablePositions.ContainsKey(nextPosition) ? WalkablePositions[nextPosition] : false);
    }
    IList<Vector2> GetWalkableNodes(Vector2 curr) //checks if the 8 nodes around unit are walkable, returns the nodes that are walkable
    {
        IList<Vector2> walkableNodes = new List<Vector2>();

        IList<Vector2> possibleNodes = new List<Vector2>()
        {
            new Vector2 (curr.x + 1, curr.y),
            new Vector2 (curr.x - 1, curr.y),
            new Vector2 (curr.x, curr.y + 1),
            new Vector2 (curr.x, curr.y - 1),
            new Vector2 (curr.x + 1, curr.y + 1),
            new Vector2 (curr.x + 1, curr.y - 1),
            new Vector2 (curr.x - 1, curr.y + 1),
            new Vector2 (curr.x - 1, curr.y - 1)
        };

        foreach (Vector2 node in possibleNodes)
        {
            if (CanMove(node))
            {
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }
    int HeuristicCostEstimate(Vector2 node, Vector2 goal, string heuristic)
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
    int EuclideanEstimate(Vector2 node, Vector2 goal)
    {
        return (int)Mathf.Sqrt(Mathf.Pow(node.x - goal.x, 2) +
            Mathf.Pow(node.y - goal.y, 2));
    }
    int ManhattanEstimate(Vector2 node, Vector2 goal)
    {
        return (int)(Mathf.Abs(node.x - goal.x) +
            Mathf.Abs(node.y - goal.y));
    }
    private int Weight(Vector2 node) //checks if the node requested is withing obstacles dictionary and returns the obstacle level. if it is not inside dictionary, returns basic level which is 1
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

    public IList<Vector2> FindShortestPath(PathfindingType algorithm, Vector3 currentPosition, Vector3 goalPosition)
    {
        Vector2 startCellId = _grid.CellIdFromPosition(currentPosition);
        Vector2 goalCellid = _grid.CellIdFromPosition(goalPosition);

        IList<Vector2> path = new List<Vector2>();
        Vector2 goal;
        switch (algorithm)
        {
            case PathfindingType.AStarEuclid:
                goal = FindShortestPathAStar(startCellId, goalCellid, "euclidean");
                break;
            default:
                goal = FindShortestPathAStar(startCellId, goalCellid, "manhattan");
                break;
        }


        if (goal == startCellId || !_nodeParents.ContainsKey(_nodeParents[goal]))
        {
            //No solution was found.
            //Debug.Log($"No pathfining found between: {currentPosition} and {goalPosition}");
            return null;
        }

        Vector2 curr = goal;
        while (curr != startCellId)
        {
            path.Add(curr);
            curr = _nodeParents[curr];
        }

        return path;
    }

    Vector2 FindShortestPathAStar(Vector2 startPosition, Vector2 goalPosition, string heuristic)
    {

        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        // A* tries to minimize f(x) = g(x) + h(x), where g(x) is the distance from the start to node "x" and
        //    h(x) is some heuristic that must be admissible, meaning it never overestimates the cost to the next node.
        //    There are formal logical proofs you can look up that determine how heuristics are and are not admissible.

        IEnumerable<Vector2> validNodes = WalkablePositions
            .Where(x => x.Value == true)
            .Select(x => x.Key);

        // Represents h(x) or the score from whatever heuristic we're using
        IDictionary<Vector2, int> heuristicScore = new Dictionary<Vector2, int>();

        // Represents g(x) or the distance from start to node "x" (Same meaning as in Dijkstra's "distances")
        IDictionary<Vector2, int> distanceFromStart = new Dictionary<Vector2, int>();

        foreach (Vector2 vertex in validNodes)
        {
            heuristicScore.Add(new KeyValuePair<Vector2, int>(vertex, int.MaxValue));
            distanceFromStart.Add(new KeyValuePair<Vector2, int>(vertex, int.MaxValue));
        }

        heuristicScore[startPosition] = HeuristicCostEstimate(startPosition, goalPosition, heuristic);
        distanceFromStart[startPosition] = 0;

        // The item dequeued from a priority queue will always be the one with the lowest int value
        //    In this case we will input nodes with their calculated distances from the start g(x),
        //    so we will always take the node with the lowest distance from the queue.
        SimplePriorityQueue<Vector2, int> priorityQueue = new SimplePriorityQueue<Vector2, int>();
        priorityQueue.Enqueue(startPosition, heuristicScore[startPosition]);

        while (priorityQueue.Count > 0)
        {
            // Get the node with the least distance from the start
            Vector2 curr = priorityQueue.Dequeue();
            nodeVisitCount++;

            // If our current node is the goal then stop
            if (curr == goalPosition)
            {
                //Debug.Log("A*" + heuristic + ": " + distanceFromStart[goalPosition]);
                //Debug.Log("A*" + heuristic + " time: " + (Time.realtimeSinceStartup - timeNow).ToString());
                //Debug.Log(string.Format("A* {0} visits: {1} ({2:F2}%)", heuristic, nodeVisitCount, (nodeVisitCount / (double)walkablePositions.Count) * 100));
                return goalPosition;
            }

            IList<Vector2> neighbors = GetWalkableNodes(curr);

            foreach (Vector2 node in neighbors)
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

        return startPosition;
    }

    public enum PathfindingType
    {
        AStarEuclid,
        AStarManhattan
    }

    public void DrawPositions()
    {
        if (WalkablePositions == null)
        {
            return;
        }

        for (int b = 0; b < WalkablePositions.Count; b++)
        {
            Gizmos.color = Color.red;
            int size = _grid.CellSize;

            var target = WalkablePositions.ElementAt(b);
            var node = _grid.GetCellPositionFromId(target.Key);
            var _moveTarget = _grid.GetCellWorldCenter(node);

            Gizmos.DrawWireCube(_moveTarget, Vector3.one * size);
        }
    }
}
