using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Grid : MonoBehaviour
{
    public bool displayGrid = true;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    private Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 -
                                  Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) +
                                     Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        
        return neighbours;
    }
    
    public List<Node> GetCardinalNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                if (x == -1 && y == -1) continue;
                if (x == 1 && y == 1) continue;
                if (x == -1 && y == 1) continue;
                if (x == 1 && y == -1) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public float CompareWithOther(Grid otherGrid)
    {
        float reward = -0.5f;
        //Debug.Log("comparing knowledge, 안녕!");
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (otherGrid.grid[x, y].state == Node.State.KNOWN
                                        && grid[x, y].state == Node.State.UNKNOWN)
                {
                    grid[x, y].state = Node.State.INFORMED;
                    reward += 0.3f;
                }
            }
        }
        return reward;
    }

    //deprecated
    public float JudgeFinalGrid()
    {
        float reward = 0f;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                switch (grid[x,y].state)
                {
                    case Node.State.KNOWN: break;//testing only negative reward
                    case Node.State.INFORMED: break;
                    case Node.State.UNKNOWN: reward -= 0.5f; break;
                    //default would be UNWALKABLE, but it yields no change so isnt written.
                }
            }
        }

        return reward;
    }
    
    public bool isFullyExplored()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x, y].state == Node.State.UNKNOWN) return false;
            }
        }
        return true;
    }

    public void ResetGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x, y].state == Node.State.KNOWN || grid[x, y].state == Node.State.INFORMED)
                {
                    grid[x, y].state = Node.State.UNKNOWN;
                }
            }
        }
    }
    
    //returns the node that the world position is inside of
    public Node NodeFromWorldPoint(Vector3 WorldPosition)
    {
        Vector2 percent = new Vector2(
            (WorldPosition.x + gridWorldSize.x/2) / gridWorldSize.x,
            (WorldPosition.z + gridWorldSize.y/2) / gridWorldSize.y);
        percent.x = Mathf.Clamp01(percent.x);
        percent.y = Mathf.Clamp01(percent.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percent.x);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percent.y);

        return grid[x, y];
    }

    [Serializable]
    public class NodeType
    {
        public LayerMask nodeMask;
        public int terrainpenalty;
    }
    
    public List<Node> path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGrid)
        {
            foreach (Node n in grid)
            {
                //set ylw if known area, else white is walkable, else red
                switch(n.state)
                {
                    case Node.State.KNOWN: Gizmos.color = Color.yellow;
                        break;
                    case Node.State.UNKNOWN: Gizmos.color = Color.white;
                        break;
                    case Node.State.UNWALKABLE: Gizmos.color = Color.blue;
                        break;
                    case Node.State.INFORMED: Gizmos.color = Color.green;
                        break;
                }
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter-0.1f));
            }
        }
    }
}
