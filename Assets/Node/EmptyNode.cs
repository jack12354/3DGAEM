using UnityEngine;
using Random = System.Random;

public class EmptyNode : Node
{
    public override void OnAdd()
    {
        IsBuildable = true;
        IsPassable = true;
    }

    public override void OnRemove()
    {
        Debug.LogWarning("You shouldn't be removing EmptyNodes from the grid, but feel free to carry on");
    }

    public override void OnEnter(Actor inActor)
    {
        // Nothing interesting happens...
    }

    void OnMouseDown()
    {
        Mouse mouse = FindObjectOfType<Mouse>();
       // for (int i = 0; i < 5; i++)
       // {
            Parent.SpawnActorsAtPoint(mouse.SpawnType, Input.mousePosition + new Vector3((float)((r.NextDouble() * 40f) - 20f), (float)((r.NextDouble() * 40f) - 20f)));
       // }
    }
}
