using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdelState : State
{
    float timer;

    public IdelState(StateAgent owner, string name) : base(owner,name)
    {

    }

    public override void OnEnter()
    {
        owner.movement.Stop();
        //timer = 2;
        //owner.timer.value = 2.0f;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <=0)
        {
            owner.stateMachine.SetState(owner.stateMachine.StateFromName("patrol"));
        }
        
    }

}
