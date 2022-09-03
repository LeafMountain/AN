using System;
using AIBehaviors;
using Core;
using Sirenix.Serialization;
using Unity.Netcode;

public class AI : ActorComponent
{
    [NonSerialized, OdinSerialize]
    public IAIBehavior[] behaviors;
    private IAIBehavior runningBehavior;

    public override void OnParentInitialized()
    {
        base.OnParentInitialized();
        
        if (NetworkManager.Singleton.IsServer == false)
        {
            enabled = false; 
        }
        
        SetBehavior(0);
    }

    public void SetBehavior(int index)
    {
        runningBehavior?.OnExit(Parent as Character);
        runningBehavior = behaviors[index];
        runningBehavior?.OnEnter(Parent as Character);
    }

    public void FixedUpdate()
    {
        runningBehavior?.OnUpdate(Parent as Character); 
    }
}
