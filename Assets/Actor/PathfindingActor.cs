using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using PathPair = System.Collections.Generic.KeyValuePair<System.Collections.Generic.List<Node>, Node>;
using PriorityQueuePair = System.Collections.Generic.KeyValuePair<int, int>;

public class PathfindingActor : Actor
{
    private List<Node> pathToGoal;
    private Vector3 randomOffset;

    void Start()
    {
        float randomAngle = (float)(r.NextDouble() * 360);
        randomOffset = new Vector3(Mathf.Sin(randomAngle) / 4.0f, Mathf.Cos(randomAngle) / 4.0f);
        //  randomOffset = new Vector3((float)((r.NextDouble() * 0.5f) - 0.25f), (float)((r.NextDouble() * 0.5f) - 0.25f));
        pathToGoal = FindPathToGoalNode(WorldGrid.GetClosestNodeFromPosition(transform.position), WorldGrid.GetGoalNode());
    }

    private List<Node> FindPathToGoalNode(Node inStartNode, Node inGoalNode)
    {
        List<Node> closedList = new List<Node>();
        List<KeyValuePair<float, PathPair>> openList = new List<KeyValuePair<float, PathPair>>();
        List<Node> tempList = new List<Node>();
        openList.Add(
            new KeyValuePair<float, PathPair>(
                Heuristic(inStartNode, inGoalNode),
                new PathPair(new List<Node> { inStartNode }, inStartNode)));

        while (openList.Count > 0)
        {
            float minNodeValue = openList.Min(n => n.Key);

            var currentNodePair = openList.FirstOrDefault(node => Math.Abs(node.Key - minNodeValue) < 0.001f);
            var currentNode = currentNodePair.Value;
            openList.Remove(currentNodePair);

            if (currentNode.Value.GetType() == typeof(GoalNode))
            {
                currentNode.Key.Add(currentNode.Value);
                return currentNode.Key;
            }

            tempList = WorldGrid.GetNeighbors(currentNode.Value);
            foreach (var node in tempList)
            {
                node.Flagged = true;
            }
            closedList.Add(currentNode.Value);

            foreach (var node in tempList)
            {
                if (node.IsPassable && !closedList.Contains(node))
                {
                    List<Node> newList = currentNode.Key.ToList();
                    newList.Add(currentNode.Value);
                    openList.Add(
                        new KeyValuePair<float, PathPair>(
                            Heuristic(node, inGoalNode) + newList.Count,
                            new PathPair(newList, node)));
                }
            }
        }
        Debug.Log("aw crud");
        Die();
        return new List<Node> { inStartNode };
    }

    private float Heuristic(Node inNode, Node inGoalNode)
    {
        if (inNode.GetType() == typeof(KillNode))
            return 9999;
        return (inNode.transform.position - inGoalNode.transform.position).magnitude + r.Next(10);
    }

    private int nodeIndex = 0;
    void Update()
    {
        Vector3 nextPosition = Vector3.Lerp(transform.position, pathToGoal[nodeIndex].transform.position + randomOffset, 0.05f);

        Node nextNode = WorldGrid.GetClosestNodeFromPosition(nextPosition);
        Node prevNode = WorldGrid.GetClosestNodeFromPosition(transform.position);

        if (nextNode != prevNode)
        {
            if (nextNode.GetType() == typeof(PortalNode))
            {
                ((PortalNode)nextNode).linkNode.OnEnter(this);
                nodeIndex++;
            }
            nextNode.OnEnter(this);
        }

        Vector3 directionToTarget = (pathToGoal[nodeIndex].transform.position + randomOffset) - transform.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;
        if (dSqrToTarget < 0.25f)
        {
            nodeIndex++;
            if (nodeIndex == pathToGoal.Count)
            {
                Win();
            }
        }

        transform.position = nextPosition;
    }

    public override void OnAdd(Grid inGrid)
    {
        WorldGrid = inGrid;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, pathToGoal[nodeIndex].transform.position + randomOffset);

        Gizmos.color = Color.green;
        for (int iter = nodeIndex; iter < pathToGoal.Count - 1; iter++)
        {
            Gizmos.DrawLine(pathToGoal[iter].transform.position + randomOffset, pathToGoal[iter + 1].transform.position + randomOffset);
        }
    }
}
