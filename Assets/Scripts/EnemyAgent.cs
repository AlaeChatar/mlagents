using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEngine.GraphicsBuffer;

public class EnemyAgent : Agent
{
    public float speedmultiplier = 0.5f;
    public float rotationmultiplier = 5;
    public GameObject agent;
    public GameObject bullet;
    private float timer;

    public override void OnEpisodeBegin()
    {
        // reset de positie en orientatie als de enemy valt
        if (this.transform.localPosition.y < 0)
        {
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            this.transform.localRotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acties, size = 2    
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedmultiplier);
        transform.Rotate(0.0f, rotationmultiplier * actionBuffers.ContinuousActions[1], 0.0f);
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, bullet.transform.localPosition);
        float distanceToSpawn = Vector3.Distance(this.transform.localPosition, new Vector3(0, 0.5f, 0));

        //// player bereikt 
        if (distanceToTarget > 1.42f)
        {
            AddReward(.5f);
            if (timer >= 4 && distanceToSpawn == 0)
            {
                AddReward(.5f);
                EndEpisode();
            }
        }
        if (distanceToTarget < 1.42f)
        {
            AddReward(-.5f);
            EndEpisode();
        }
        
        // Van het platform gevallen?    
        if (this.transform.localPosition.y < 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent positie   
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.agent.transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
    }
}