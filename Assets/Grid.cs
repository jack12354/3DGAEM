using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Grid : MonoBehaviour
{
    public static int ROWS = 9;
    public static int COLUMNS = 17;
    public const float GRID_SIZE = 1.0f;
    public string LevelToLoad = "Levels/world1-1";

    public Node[,] mGrid;
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
        ROWS = level.Count(x => x == '\r');
        COLUMNS = level.IndexOf('\r');
        mGrid = new Node[ROWS, COLUMNS];
        level = level.Replace("\r\n", "");
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLUMNS; col++)
            {
                char tile = level[row * COLUMNS + col];
                if (tile >= '0' && tile <= '9')
                {
                    if (portalChannels.ContainsKey(tile))
                    {
                        PortalNode node = AddNode(charToNodeMap['p'], row, col) as PortalNode;
                        node.Link(portalChannels[tile]);
                    }
                    else
                    {
                        PortalNode node = AddNode(charToNodeMap['p'], row, col) as PortalNode;
                        portalChannels[(int)tile] = node;
                    }
                }
                else
                {
                    AddNode(charToNodeMap[tile], row, col);
                }
            }
        }
    }

    private Node AddNode(string inNodeType, int inRow, int inCol)
    {
        mGrid[inRow, inCol] = ((GameObject)Instantiate(Resources.Load(inNodeType))).GetComponent<Node>();
        mGrid[inRow, inCol].OnAdd();
        mGrid[inRow, inCol].Parent = this;
        mGrid[inRow, inCol].transform.position = GetGridPosition(inRow, inCol);
        return mGrid[inRow, inCol];
    }

    public Node GetClosestNodeFromPosition(Vector3 inPosition)
    {
        Node bestNode = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var node in mGrid)
        {
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
        Node bestNode = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var node in mGrid)
        {
            Vector3 directionToTarget = node.transform.position - inPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestNode = node;
            }
        }

        return bestNode.IsPassable;
    }

    public void SpawnActorsAtPoint(string inActorType, Vector3 inMousePosition)
    {
        Actor actor = ((GameObject)Instantiate(Resources.Load(inActorType))).GetComponent<Actor>();
        Vector3 screenToWorldPos = Camera.main.ScreenToWorldPoint(inMousePosition);
        screenToWorldPos.z = 0;
        actor.transform.position = screenToWorldPos;
        actor.OnAdd(this);
        mActors.Add(actor);
    }

    public List<Node> GetNeighbors(Node inNode)
    {
        List<Node> nodes = new List<Node>();
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLUMNS; col++)
            {
                if (inNode == mGrid[row, col])
                {
                    if (row - 1 >= 0)
                    {
                        nodes.Add(AddNeighborNode(row - 1, col));
                    }
                    if (row + 1 < ROWS)
                    {
                        nodes.Add(AddNeighborNode(row + 1, col));
                    }
                    if (col - 1 >= 0)
                    {
                        nodes.Add(AddNeighborNode(row, col - 1));
                    }
                    if (col + 1 < COLUMNS)
                    {
                        nodes.Add(AddNeighborNode(row, col + 1));
                    }

                    if (inNode.GetType() == typeof(PortalNode))
                    {
                        nodes.Add(((PortalNode)inNode).linkNode);
                    }

                    return nodes;
                }
            }
        }
        return nodes;
    }

    private Node AddNeighborNode(int row, int col)
    {
        Node node = mGrid[row, col];
       //if (node.GetType() == typeof(PortalNode))
       //{
       //    return ((PortalNode)node).linkNode;
       //}
        return node;
    }

    public Vector3 GetGridPosition(int inRow, int inCol)
    {
        return transform.position + new Vector3((inCol + 0.5f) * GRID_SIZE, (ROWS - inRow - 0.5f) * GRID_SIZE)
                             - new Vector3(COLUMNS * GRID_SIZE / 2.0f, ROWS * GRID_SIZE / 2.0f);
    }


    // Update is called once per frame
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
