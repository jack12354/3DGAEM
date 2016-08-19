﻿public class GoalNode : Node
{
    public override void OnAdd()
    {
        IsPassable = true;
        IsBuildable = false;
    }

    public override void OnRemove()
    {
        Destroy(gameObject);
    }

    public override void OnEnter(Actor inActor)
    {

    }
}
