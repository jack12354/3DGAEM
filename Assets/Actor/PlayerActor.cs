using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerActor : Actor
{
    public override void OnAdd(Grid inGrid)
    {
        WorldGrid = inGrid;
    }

    void Update()
    {
        heading = Vector3.zero;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            heading += Vector3.down;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            heading += Vector3.up;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            heading += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            heading += Vector3.right;
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            heading += Vector3.back;
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            heading += Vector3.forward;
        }

        heading.Normalize();

        if (WorldGrid.GetClosestNodeFromPosition(transform.position + (heading * Time.deltaTime * 2.0f)).IsPassable)
        {
            transform.position += heading * Time.deltaTime * 2.0f;
        }

        Node nextNode = WorldGrid.GetClosestNodeFromPosition(transform.position + (heading * Time.deltaTime * 2.0f));
        Node prevNode = WorldGrid.GetClosestNodeFromPosition(transform.position);
        if (nextNode != prevNode)
        {
            nextNode.OnEnter(this);
        }

        if(prevNode.GetType() == typeof(GoalNode))
            Win();

    }

    public override void OnRemove()
    {
        Destroy(gameObject);
    }

    public override void Win()
    {
        Destroy(Instantiate(Resources.Load("WinEffect"), transform.position, Quaternion.Euler(0, -180, -180)), 0.5f);
        OnRemove();
    }

    public override void Die()
    {
        Destroy(Instantiate(Resources.Load("DeathEffect"), transform.position, Quaternion.Euler(0, -180, -180)), 0.5f);
        OnRemove();
    }
}
