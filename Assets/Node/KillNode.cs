public class KillNode : Node
{
    public override void OnAdd()
    {
        IsBuildable = false;
        IsPassable = true;
    }

    public override void OnRemove()
    {
        Destroy(gameObject);
    }

    public override void OnEnter(Actor inActor)
    {
        inActor.Die();
    }
}

