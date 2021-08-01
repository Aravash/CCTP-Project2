using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class socialAgentMapper : Agent
{
    private agentMovement moveHandler;
    [SerializeField] private GameObject environment;
    [SerializeField] private agentList agent_list;
    [SerializeField] private Color trail_color;
    
    public override void Initialize()
    {
        agent_list = GetComponentInParent<agentList>();
        moveHandler = GetComponent<agentMovement>();
    }

    public override void OnEpisodeBegin()
    {
        //random position on start
        transform.localPosition = new Vector3(Random.Range(-10, 10), 1.5f, Random.Range(-10, 10));
        
        foreach (var child in environment.GetComponentsInChildren<PointKnowledge>())
        {
            child.resetState();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        foreach (Transform tr in agent_list.agentPositions)
        {   
            sensor.AddObservation(tr.localPosition);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var input = actions.DiscreteActions;

        switch (input[0])
        {
            case 0://move up
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,1), trail_color));
                break;
            case 1://move down
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,-1), trail_color));
                break;
            case 2: //move right
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(1,0,0), trail_color));
                break;
            case 3://move left
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(-1,0,0), trail_color));
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            action[0] = 3;
            //move right
            Debug.Log("D key pressed");
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(1,0,0), trail_color));
        }
        else if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
            //move up
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,1), trail_color));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            action[0] = 4;
            //move left
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(-1,0,0), trail_color));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2;
            //move down
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,-1), trail_color));
        }
    }
}
