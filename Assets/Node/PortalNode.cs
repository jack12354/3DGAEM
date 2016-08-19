using UnityEngine;
using System.Collections;

public class PortalNode : Node
{
    public PortalNode linkNode;
    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    public void Link(PortalNode inPortalNode)
    {
        linkNode = inPortalNode;
        linkNode.linkNode = this;
    }

    public void UnLink()
    {
        linkNode.linkNode = null;
        linkNode = null;
    }


    public override void OnAdd()
    {
        IsBuildable = false;
        IsPassable = true;
    }

    public override void OnRemove()
    {

    }

    public override void OnEnter(Actor inActor)
    {
        inActor.transform.position = linkNode.transform.position + new Vector3((float)((r.NextDouble() * 0.66f) - 0.33f), (float)((r.NextDouble() * 0.66f) - 0.33f));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, linkNode.transform.position);
    }
}
