using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomusAgent : Agent
{
    [SerializeField] Perception perception;
    [SerializeField] Steering steering;
    [SerializeField] ObsicalPerception obstaclePerception;
    [SerializeField] Perception flockPerception;
    [SerializeField] AutonomousAgentData agentData;
    // Update is called once per frame
    void Update()
    {

        GameObject[] gameObjects = perception.GetGameObjects();
        if(gameObjects.Length ==0)
        {
             movement.ApplyForce(steering.Wander(this));
        }
        //seek //flee
        if(gameObjects.Length != 0)
        {
            movement.ApplyForce(steering.Seek(this, gameObjects[0])*agentData.seekWeight);
            movement.ApplyForce(steering.Flee(this, gameObjects[0])*agentData.fleeWeight);
        }
        //flcoking
        gameObjects = flockPerception.GetGameObjects();
        if(gameObjects.Length != 0)
        {
            movement.ApplyForce(steering.Cohesion(this, gameObjects)*agentData.cohesionWeight);
            movement.ApplyForce(steering.Seperation(this, gameObjects, agentData.separationRadius) * agentData.separationWeight);
            movement.ApplyForce(steering.Alignment(this, gameObjects) * agentData.alignmentWeight);
        }
        //obstacle avoidance
        if (obstaclePerception.IsObstacleInFront())
        {
            Vector3 direction = obstaclePerception.GetOpenDirection();
            movement.ApplyForce(steering.CalculateSteering(this, direction) * agentData.obstacleWeight);
        }



    }
}
