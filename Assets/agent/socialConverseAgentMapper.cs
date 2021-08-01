using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class socialConverseAgentMapper : Agent
{
    public Grid grid;

    private gridAgentMovement moveHandler;
    [SerializeField] private int my_agent_list_pos = 0;
    [SerializeField] private agentList agent_list;
    [SerializeField] private Color trail_color;
    
    [SerializeField] private GameObject explorerMap;

    private bool has_achieved_objective;
    
    public override void Initialize()
    {
        agent_list = GetComponentInParent<agentList>();
        moveHandler = GetComponent<gridAgentMovement>();
        grid = Instantiate(explorerMap, Vector3.zero, Quaternion.identity, agent_list.transform).GetComponent<Grid>();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode has begun for agent "+ my_agent_list_pos);
        
        //random position on start
        transform.localPosition = new Vector3(Random.Range(-10, 10), 1.5f, Random.Range(-10, 10));

        has_achieved_objective = false;
        
        //call grid reset function
        grid.ResetGrid();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);//separate your position from the list of agentPositions;

        int i = 0;
        foreach (Transform tr in agent_list.agentPositions)
        {
            if (i != my_agent_list_pos) sensor.AddObservation(tr.localPosition);
            i++;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (has_achieved_objective) return;
        
        var input = actions.DiscreteActions;

        switch (input[0])
        {
            case 0://move up
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,1), trail_color, grid));
                break;
            case 1://move down
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,-1), trail_color, grid));
                break;
            case 2: //move right
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(1,0,0), trail_color, grid));
                break;
            case 3://move left
                AddReward(moveHandler.moveAndRewardAgent(new Vector3(-1,0,0), trail_color, grid));
                break;
            case 4:
                Transform closestagent = agent_list.agentPositions.Find(tr => Vector3.Distance(transform.position, tr.position) <= 0.8);
                AddReward(moveHandler.tryConverseWithOthers(grid, closestagent));
                break;
        }

        if (grid.isFullyExplored())
        {
            AddReward(MaxStep - StepCount); //greater reward for faster completion
            Debug.Log("Map has been completed by agent " + my_agent_list_pos);
            has_achieved_objective = true;
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
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(1,0,0), trail_color, grid));
        }
        else if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
            //move up
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,1), trail_color, grid));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            action[0] = 4;
            //move left
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(-1,0,0), trail_color, grid));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2;
            //move down
            AddReward(moveHandler.moveAndRewardAgent(new Vector3(0,0,-1), trail_color, grid));
        }
    }
}
