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
        Destroy(gameObject);
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Parent.SpawnActorsAtPoint(mouse.SpawnType, hit.point
           //  + new Vector3(
           //  (float)((r.NextDouble() * 1f) - 0.5f), 
           //  (float)((r.NextDouble() * 1f) - 0.5f), 
           //  (float)((r.NextDouble() * 1f) - 0.5f))
           );
        }

       
       // }
    }
}
