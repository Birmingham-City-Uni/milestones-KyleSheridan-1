using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;


//Sebastian Lague - Pathfinding https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
public class Pathfinding : MonoBehaviour
{
    Grid grid;

    public enum Algorithm
    {
        AStar,
        UniformCost,
        BestFirst
    }

    public Algorithm pathfindingAlgorithm = Algorithm.AStar;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

        if(startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if(currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }

                foreach(Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCost;

                    switch (pathfindingAlgorithm)
                    {
                        case Algorithm.AStar:
                            newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                            if(newMovementCost < neighbour.gCost || !openSet.Contains(neighbour))
                            {
                                neighbour.gCost = newMovementCost;
                                neighbour.hCost = GetDistance(neighbour, targetNode);

                                neighbour.parent = currentNode;

                                if (!openSet.Contains(neighbour))
                                {
                                    openSet.Add(neighbour);
                                }
                                else
                                {
                                    openSet.UpdateItem(neighbour);
                                }
                            }
                            break;
                        case Algorithm.UniformCost:
                            newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                            if (newMovementCost < neighbour.gCost || !openSet.Contains(neighbour))
                            {
                                neighbour.gCost = newMovementCost;

                                neighbour.parent = currentNode;

                                if (!openSet.Contains(neighbour))
                                {
                                    openSet.Add(neighbour);
                                }
                                else
                                {
                                    openSet.UpdateItem(neighbour);
                                }
                            }
                            break;
                        case Algorithm.BestFirst:
                            newMovementCost = currentNode.hCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                            if (newMovementCost < neighbour.hCost || !openSet.Contains(neighbour))
                            {
                                neighbour.hCost = GetDistance(neighbour, targetNode);

                                neighbour.parent = currentNode;

                                if (!openSet.Contains(neighbour))
                                {
                                    openSet.Add(neighbour);
                                }
                                else
                                {
                                    openSet.UpdateItem(neighbour);
                                }
                            }
                            break;
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i - 1].worldPos);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
        //return (int) Mathf.Sqrt(distX * distX + distY * distY);
    }
}
