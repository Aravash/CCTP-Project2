using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum State
    {
        UNKNOWN,
        UNWALKABLE,
        KNOWN,
        INFORMED
    }

    public State state = State.UNKNOWN;
    public Vector3 worldPos;
    public int gridX, gridY;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        if (!_walkable) state = State.UNWALKABLE;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}
