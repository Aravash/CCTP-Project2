using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agentList : MonoBehaviour
{
    public List<Transform> agentPositions;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Transform tr in transform)
        {
            agentPositions.Add(tr);
        }
    }
}
