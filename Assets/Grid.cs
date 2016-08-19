using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Grid : MonoBehaviour
{
    public static int ROWS = 9;
    public static int COLUMNS = 17;
    public static int PLANES = 3;
    public const float GRID_SIZE = 1.0f;
    public string LevelToLoad = "Levels/world1-1";

    public Node[,,] mGrid;
    private List<Actor> mActors = new List<Actor>();
    private readonly Dictionary<char, string> charToNodeMap = new Dictionary<char, string>
    {
        {'w', "WallNode"},
        {'g', "GoalNode"},
        {'p', "PortalNode"}, // channel specified with 0-9
        {'x', "KillNode" },
        {' ', "EmptyNode"}
    };


    public Random random = new Random();

    void Start()
    {
        Load(LevelToLoad);
    }

    public void Load(string inFilename)
    {
        Dictionary<int, PortalNode> portalChannels = new Dictionary<int, PortalNode>();

        string level = Resources.Load<TextAsset>(inFilename).text;
        COLUMNS = level.IndexOf('\r');
        PLANES = level.Count(x => x == '-');
        ROWS = level.Count(x => x == '\r') / PLANES;
        mGrid = new Node[ROWS, COLUMNS, PLANES];
        level = level.Replace("\r\n", "");
        level = level.Replace("-", "");
        for (int plane = 0; plane < PLANES; plane++)
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    char tile = level[plane*ROWS*COLUMNS + row*COLUMNS + col];
                    if (tile >= '0' && tile <= '9')
                    {
                        if (portalChannels.ContainsKey(tile))
                        {
                            PortalNode node = AddNode(charToNodeMap['p'], row, col, plane) as PortalNode;
                            node.Link(portalChannels[tile]);
                        }
                        else
                        {
                            PortalNode node = AddNode(charToNodeMap['p'], row, col, plane) as PortalNode;
                            portalChannels[(int) tile] = node;
                        }
                    }
                    else
                    {
                        AddNode(charToNodeMap[tile], row, col, plane);
                    }
                }
            }
        }
    }

    private Node AddNode(string inNodeType, int inRow, int inCol, int inPlane)
    {
        mGrid[inRow, inCol, inPlane] = ((GameObject)Instantiate(Resources.Load(inNodeType))).GetComponent<Node>();
        mGrid[inRow, inCol, inPlane].OnAdd();
        mGrid[inRow, inCol, inPlane].Parent = this;
        mGrid[inRow, inCol, inPlane].transform.position = GetGridPosition(inRow, inCol, inPlane);
        mGrid[inRow, inCol, inPlane].name = inNodeType + "(" + inRow + ", " + inCol + ", " + inPlane + ")";
        mGrid[inRow, inCol, inPlane].transform.parent = transform;
        return mGrid[inRow, inCol,inPlane];
    }

    private void RemoveNode(Node inNode)
    {
        inNode.OnRemove();
        Vector3 coordinates = GetNodeCoordinates(inNode);
        mGrid[(int) coordinates.x, (int) coordinates.y, (int) coordinates.z] = null;
    }

    public Node ReplaceNode(Node inNode, string inReplaceType)
    {
        Debug.Log(inNode.GetType() + " -> " + inReplaceType);
        Vector3 coordinates = GetNodeCoordinates(inNode);
        RemoveNode(inNode);
        return AddNode(inReplaceType, (int)coordinates.x, (int)coordinates.y, (int)coordinates.z);
    }

    public Node GetClosestNodeFromPosition(Vector3 inPosition, bool inPassableOnly = false)
    {
        Node bestNode = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var node in mGrid)
        {
            if (inPassableOnly && !node.IsPassable)
                continue;

            Vector3 directionToTarget = node.transform.position - inPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestNode = node;
            }
        }

        return bestNode;
    }

    public bool IsPointPassable(Vector3 inPosition)
    {
        return GetClosestNodeFromPosition(inPosition).IsPassable;
    }

    public bool IsPointWithinGrid(Vector3 inPosition)
    {
        Vector3 zerozerozero = GetGridPosition(0, 0, 0);
        Vector3 maxmaxmax = GetGridPosition(ROWS - 1, COLUMNS - 1, PLANES - 1);
        return
            inPosition.x >= zerozerozero.x && inPosition.x <= maxmaxmax.x &&
            inPosition.y <= zerozerozero.y && inPosition.y >= maxmaxmax.y &&
            inPosition.z >= zerozerozero.z && inPosition.z <= maxmaxmax.z;
    }

    public void SpawnActorsAtPoint(string inActorType, Vector3 inMousePosition)
    {
        Actor actor = ((GameObject)Instantiate(Resources.Load(inActorType))).GetComponent<Actor>();
        Vector3 screenToWorldPos = Camera.main.ScreenToWorldPoint(inMousePosition);
        actor.transform.position = inMousePosition;//screenToWorldPos;
        actor.OnAdd(this);
        mActors.Add(actor);
    }

    public void RemoveActor(Actor inActor)
    {
        mActors.Remove(inActor);
    }

    public List<Actor> GetAllActors()
    {
        return mActors;
    }

    public List<Actor> GetAllActorsOfType<T>()
    {
        return mActors.Where(a => a.GetType() == typeof(T)).ToList();
    }

    public List<Node> GetNeighbors(Node inNode)
    {
        List<Node> nodes = new List<Node>();
        for (int pln = 0; pln < PLANES; pln++)
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    if (inNode == mGrid[row, col, pln])
                    {
                        if (row - 1 >= 0)
                        {
                            nodes.Add(AddNeighborNode(row - 1, col, pln));
                        }
                        if (row + 1 < ROWS)
                        {
                            nodes.Add(AddNeighborNode(row + 1, col, pln));
                        }
                        if (col - 1 >= 0)
                        {
                            nodes.Add(AddNeighborNode(row, col - 1, pln));
                        }
                        if (col + 1 < COLUMNS)
                        {
                            nodes.Add(AddNeighborNode(row, col + 1, pln));
                        }
                        if (pln - 1 >= 0)
                        {
                            nodes.Add(AddNeighborNode(row, col, pln - 1));
                        }
                        if (pln + 1 < PLANES)
                        {
                            nodes.Add(AddNeighborNode(row, col, pln + 1));
                        }

                        if (inNode.GetType() == typeof(PortalNode))
                        {
                            nodes.Add(((PortalNode) inNode).linkNode);
                        }

                        return nodes;
                    }
                }
            }
        }
        return nodes;
    }

    private Node AddNeighborNode(int row, int col, int pln)
    {
        Node node = mGrid[row, col, pln];
       //if (node.GetType() == typeof(PortalNode))
       //{
       //    return ((PortalNode)node).linkNode;
       //}
        return node;
    }

    public Vector3 GetGridPosition(int inRow, int inCol, int inPln)
    {
        return transform.position + new Vector3((inCol + 0.5f) * GRID_SIZE, (ROWS - inRow - 0.5f) * GRID_SIZE, (inPln) * GRID_SIZE)
                             - new Vector3(COLUMNS * GRID_SIZE / 2.0f, ROWS * GRID_SIZE / 2.0f, PLANES * GRID_SIZE / 2.0f);
    }

    public Vector3 GetNodeCoordinates(Node inNode)
    {
        for (int pln = 0; pln < PLANES; pln++)
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    if (inNode == mGrid[row, col, pln])
                    {
                        return new Vector3(row, col, pln);
                    }
                }
            }
        }

        return -Vector3.one;
    }

    void Update()
    {

    }

    void OnDrawGizmos()
    {
        //  foreach (var node in mGrid)
        //  {
        //      if (node.Flagged)
        //      {
        //          Gizmos.DrawCube(node.transform.position, Vector3.one * 0.5f);
        //      }
        //  }
        // foreach (var node in mGrid)
        // {
        //     if (node.IsPassable)
        //     {
        //         Gizmos.color = Color.green;
        //         Gizmos.DrawCube(node.transform.position, Vector3.one * 1f);
        //     }
        //     else
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawCube(node.transform.position, Vector3.one * 1f);
        //     }
        // }
    }

    public Node GetGoalNode()
    {
        return mGrid.Cast<Node>().FirstOrDefault(node => node.GetType() == typeof(GoalNode));
    }
}
