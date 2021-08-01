using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agentMovement : MonoBehaviour
{
    private int wallmask = 1 << 8;
    private int layermask = 1 << 9;
    private int agentmask = 1 << 7;
    
    public float moveAndRewardAgent(Vector3 change, Color colour)
    {
        Vector3 originalpos = transform.position;
        transform.position += change;
        
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity,
            layermask))
        {
            hit.transform.GetComponent<PointKnowledge>().makeKnown(colour);
            return 0.5f;
        }
        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity,
            wallmask))
        {
            transform.position = originalpos;
            return -5.0f;
        }
        return -3f;
    }
}
