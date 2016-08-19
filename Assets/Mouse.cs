using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour
{
    public string SpawnType = "BrainyActor";
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSpawningType(string inTypeString)
    {
        SpawnType = inTypeString;
    }
}
