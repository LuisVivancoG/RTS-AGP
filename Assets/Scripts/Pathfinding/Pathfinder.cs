using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Pathfinder
{
    public Pathfinder(GameGrid grid, Dictionary<Vector3, GridCell> cells)
    {
        _nodeParents = new Dictionary<Vector3, Vector3>();
        _walkablePositions = new Dictionary<Vector3, bool>();
        _obstacles = new Dictionary<Vector3, int>();
        _grid = grid;
        UpdateWalkableObstacles(cells);
    }

    public IDictionary<Vector3, bool> _walkablePositions;
    public IDictionary<Vector3, int> _obstacles;

    Dictionary<Vector3, Vector3> _nodeParents;
    private GameGrid _grid;

    [SerializeField] private Transform _currentPosition;
    [SerializeField] private Transform _objective;
    private List<Vector3> _currentPathFound = new List<Vector3>();
    private int _currentPathIndex;
    [SerializeField] private float _movementSpeed = 10;

    /*private void Start()
    {
        int w = 30;
        int h = 30;
        int size = 30;
        float halfSize = size / 2f;

        float posX = 0 - ((w * size) + halfSize);
        for (int i = -w; i <= w + 1; i++)
        {
            float posZ = 0 - ((h * size) + halfSize);
            for (int j = -h; j <= h + 1; j++)
            {
                posZ += size;
                Vector3 newPosition = new Vector3(i, 0, j);

                _walkablePositions.Add(new KeyValuePair<Vector3, bool>(newPosition, true));
            }
            posX += size;
        }
    } */

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //    int w = 30;
    //    int h = 30;
    //    int size = 1;
    //    float halfSize = size / 2f;

    //    float posX = 0 - ((w * size) + halfSize);
    //    for (int i = -w; i <= w + 1; i++)
    //    {
    //        float posZ = 0 - ((h * size) + halfSize);
    //        for (int j = -h; j <= h + 1; j++)
    //        {
    //            posZ += size;
    //            Gizmos.DrawWireCube((new Vector3(posX, 0, posZ)), Vector3.one * size);
    //        }
    //        posX += size;
    //    }
    //}

    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    foreach (var entry in walkablePositions)
        //    {
        //        Debug.Log($"Position: {entry.Key}, Is Walkable: {entry.Value}");
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.E))
        {
            FindShortestPathAStar(_currentPosition.position, _objective.position, "manhattan");
            _currentPathIndex = 0;
        }
        if (_currentPathFound != null && _currentPathIndex < _currentPathFound.Count)
        {
            Vector3 targetPos = _currentPathFound[_currentPathIndex];
            _currentPosition.position = Vector3.MoveTowards(_currentPosition.position, targetPos, _movementSpeed * Time.deltaTime);

            if (Vector3.Distance(_currentPosition.position, targetPos) < 0.1f)
            {
                _currentPathIndex++;
            }
        }
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

    List<Vector3> FindShortestPathAStar(Vector3 startPosition, Vector3 goalPosition, string heuristic)
    {
        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        IEnumerable<Vector3> validNodes = _walkablePositions.Where(x => x.Value == true).Select(x => x.Key);

        IDictionary<Vector3, int> heuristicScore = new Dictionary<Vector3, int>();

        // Represents g(x) or the distance from start to node "x" (Same meaning as in Dijkstra's "distances")
        IDictionary<Vector3, int> distanceFromStart = new Dictionary<Vector3, int>();

        foreach (Vector3 vertex in validNodes)
        {
            heuristicScore.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
            distanceFromStart.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue)); //?
        }

        heuristicScore[startPosition] = HeuristicCostEstimate(startPosition, goalPosition, heuristic);
        distanceFromStart[startPosition] = 0;

        SimplePriorityQueue<Vector3, int> priorityQueue = new SimplePriorityQueue<Vector3, int>();
        priorityQueue.Enqueue(startPosition, heuristicScore[startPosition]);

        while (priorityQueue.Count > 0)
        {
            // Get the node with the least distance from the start
            Vector3 curr = priorityQueue.Dequeue();
            nodeVisitCount++;

            // If our current node is the goal then stop
            if (curr == goalPosition)
            {
                return ReconstructPath(_nodeParents, goalPosition);
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

        return new List<Vector3>();
    }

    List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> nodeParents, Vector3 goalPosition)
    {
        Vector3 current = goalPosition;

        while (nodeParents.ContainsKey(current))
        {
            _currentPathFound.Add(current);
            current = nodeParents[current];
        }

        _currentPathFound.Reverse(); // Reverse list order

        return _currentPathFound;
    }

    private int Weight(Vector3 node)
    {
        if (_obstacles.Keys.Contains(node))
        {
            return _obstacles[node];
        }
        else
        {
            return 1;
        }
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

    bool CanMove(Vector3 nextPosition)
    {
        return (_walkablePositions.ContainsKey(nextPosition) ? _walkablePositions[nextPosition] : false);
    }

    public void UpdateWalkableObstacles(Dictionary<Vector3, GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (!_walkablePositions.ContainsKey(cell.Key))
            {
                _walkablePositions.Add(cell.Key, true);
            }
            _walkablePositions[cell.Key] = cell.Value.Walkable;

            if (!_walkablePositions.ContainsKey(cell.Key))
            {
                _walkablePositions.Add(cell.Key, true);
            }
            _walkablePositions[cell.Key] = cell.Value.Walkable;

            if (!_obstacles.ContainsKey(cell.Key))
            {
                _obstacles.Add(cell.Key, 1);
            }
            _obstacles[cell.Key] = cell.Value.ObstacleLevel;
        }
    }
}
