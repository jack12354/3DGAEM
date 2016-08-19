public class GoalNode : Node
{
    public override void OnAdd()
    {
        IsPassable = true;
        IsBuildable = false;
    }

    public override void OnRemove()
    {

    }

    public override void OnEnter(Actor inActor)
    {

    }
}
