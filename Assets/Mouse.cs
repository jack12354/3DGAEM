using UnityEngine;
using System.Collections;
using Random = System.Random;

public class Mouse : MonoBehaviour
{
    Random r = new Random();
    public string SpawnType = "BrainyActor";
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (SpawnType.EndsWith("Actor"))
                {
                    Node hitNode = hit.transform.gameObject.GetComponent<Node>();
                    if (hitNode)
                    {
                        Vector3 spawnPoint =
                            hitNode.Parent.GetClosestNodeFromPosition(hit.point, true).transform.position;
                        hitNode.Parent.SpawnActorsAtPoint(SpawnType, spawnPoint
                                                                           + new Vector3(
                                                                               (float)((r.NextDouble() * 1f) - 0.5f),
                                                                               (float)((r.NextDouble() * 1f) - 0.5f),
                                                                               (float)((r.NextDouble() * 1f) - 0.5f))
                        );
                    }
                }
                else if (SpawnType.EndsWith("Node"))
                {
                    Node hitNode = hit.transform.gameObject.GetComponent<Node>();
                    if (hitNode)
                    {
                        Grid grid = hitNode.Parent;
                        if (SpawnType == "DeleteNode")
                        {
                            grid.ReplaceNode(hitNode, "EmptyNode");
                        }
                        else
                        {
                            var emptySpacePoint = hit.point + hit.normal * (Grid.GRID_SIZE / 2.0f);
                            if (grid.IsPointWithinGrid(emptySpacePoint))
                            {
                                Node nodeToReplace = grid.GetClosestNodeFromPosition(emptySpacePoint);
                                grid.ReplaceNode(nodeToReplace, SpawnType);
                            }
                        }
                    }
                }
            }

        }
    }

    public void SetSpawningType(string inTypeString)
    {
        SpawnType = inTypeString;
    }
}
