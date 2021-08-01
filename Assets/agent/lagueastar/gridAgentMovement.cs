using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridAgentMovement : MonoBehaviour
{ 
    private int wallmask = 1 << 8;
    private int layermask = 1 << 9;
    private int agentmask = 1 << 7;
    
    public float moveAndRewardAgent(Vector3 change, Color colour, Grid gr)
    {
        Vector3 originalpos = transform.position;
        transform.position += change;

        Node n = gr.NodeFromWorldPoint(transform.position);
        float reward;
        switch (n.state)
        {
            case Node.State.UNKNOWN:
                n.state = Node.State.KNOWN;
                reward = 0.5f;
                break;
            case Node.State.UNWALKABLE:
                reward = -5f;
                transform.position = originalpos;
                break;
            default:
                reward = -0.5f; //if it's known or informed
                break;
        }
        return reward;
    }

    public float tryConverseWithOthers(Grid gr, Transform other)
    {
        return gr.CompareWithOther(other.GetComponent<socialConverseAgentMapper>().grid);
    }

    public float TryJudgeFinalGrid(Grid gr)
    {
        return gr.JudgeFinalGrid();
    }
}