﻿using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class KartAgent : Agent
{
   public CheckpointManager _checkpointManager;
   private KartController _kartController;
   
   //called once at the start
   public override void Initialize()
   {
      _kartController = GetComponent<KartController>();
   }
   
   //Called each time it has timed-out or has reached the goal
   public override void OnEpisodeBegin()
   {
      _checkpointManager.ResetCheckpoints();
      _kartController.Respawn();
   }



     //Collecting extra Information that isn't picked up by the RaycastSensors
      public override void CollectObservations(VectorSensor sensor)
      {
          //Vector between kart and next checkpoint
         Vector3 diff = _checkpointManager.nextCheckPointToReach.transform.position - transform.position;
         sensor.AddObservation(diff / 20f); //Divide by 20 to normalize

         AddReward(-0.001f); //Promote faster driving
      }

      //Processing the actions received
      public override void OnActionReceived(ActionBuffers actions)
      {
          var input = actions.ContinuousActions;

          _kartController.ApplyAcceleration(input[1]);
          _kartController.Steer(input[0]);
          _kartController.AnimateKart(input[0]);
      }
      
      //For manual testing with human input, the actionsOut defined here will be sent to OnActionRecieved
      public override void Heuristic(in ActionBuffers actionsOut)
      {
          var action = actionsOut.ContinuousActions;
          action.Clear();

          action[0] = Input.GetAxis("Horizontal"); //Steering

          action[1] = Input.GetKey(KeyCode.W) ? 1f : 0f; //Accelerating
      }
      
}
