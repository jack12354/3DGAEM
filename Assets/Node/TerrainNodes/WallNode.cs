public class WallNode : Node
{
    public override void OnAdd()
    {
        IsBuildable = false;
        IsPassable = false;
    }

    public override void OnRemove()
    {

    }

    public override void OnEnter(Actor inActor)
    {

    }
}
