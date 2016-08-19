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
            Mouse mouse = FindObjectOfType<Mouse>();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Node hitNode = hit.transform.gameObject.GetComponent<Node>();
                if (hitNode)
                {
                    Vector3 spawnPoint = hitNode.Parent.GetClosestNodeFromPosition(hit.point, true).transform.position;
                    hitNode.Parent.SpawnActorsAtPoint(mouse.SpawnType, spawnPoint
                         + new Vector3(
                         (float)((r.NextDouble() * 1f) - 0.5f), 
                         (float)((r.NextDouble() * 1f) - 0.5f), 
                         (float)((r.NextDouble() * 1f) - 0.5f))
                    );
                }
            }
        }
    }

    public void SetSpawningType(string inTypeString)
    {
        SpawnType = inTypeString;
    }

    void OnMouseDown()
    {
      


        // }
    }
}
