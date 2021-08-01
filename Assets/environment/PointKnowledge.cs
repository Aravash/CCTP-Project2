using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointKnowledge : MonoBehaviour
{

    [SerializeField] private Material knownMat;
    private Material unknownMat;

    private void Start()
    {
        unknownMat = GetComponent<Renderer>().material;
    }

    public void resetState()
    {
        gameObject.tag = "unknown";
        GetComponent<Renderer>().material = unknownMat;
    }
    
    public void makeKnown(Color colour) //custom colour not working rn
    {
        gameObject.tag = "known";
        GetComponent<Renderer>().material = knownMat;
    }
}
