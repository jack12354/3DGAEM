using UnityEngine;
using System.Collections;

public class TurretNode : Node
{

    public string BulletType = "SimpleBullet";
    public float RoF = 0.001f;
    private float last = 0;
    private Vector3 heading = Vector3.left;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        foreach (var actor in Parent.GetAllActors())
        {
            if (Physics.Linecast(transform.position, actor.transform.position, out hit))
            {
                if (hit.transform.gameObject == actor.gameObject &&  Time.time > last + RoF)
                {
                    GameObject bullet =
                        Instantiate(Resources.Load(BulletType), transform.position, Quaternion.identity) as
                            GameObject;
                    bullet.transform.LookAt(actor.transform.position);
                    last = Time.time;
                    heading.x = (float) ((r.NextDouble()*2.0f) - 1.0f);
                    heading.y = (float) ((r.NextDouble()*2.0f) - 1.0f);
                    heading.z = (float) ((r.NextDouble()*2.0f) - 1.0f);
                    heading.Normalize();
                    Destroy(bullet, 10);
                }
            }
        }
       

    }

    public override void OnAdd()
    {
        IsPassable = false;
        IsBuildable = false;
    }

    public override void OnRemove()
    {

    }

    public override void OnEnter(Actor inActor)
    {
        inActor.Die();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var actor in Parent.GetAllActors())
        {
            Gizmos.DrawLine(transform.position, actor.transform.position);
        }
    }
}
