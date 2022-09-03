using AIBehaviors;
using UnityEngine;

public class WanderBehavior : IAIBehavior
{
    public float distance = 1f;
    
    public void OnEnter(Character actor)
    {
        Debug.Log("Entering Wander Behavior");
    }

    public void OnExit(Character actor)
    {
        Debug.Log("Exiting Wander Behavior");
    }

    public void OnUpdate(Character actor)
    {
        if(actor.destinationReached == false) return;
        actor.SetDestination(actor.transform.position + Random.insideUnitSphere * distance);
    }
}