using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private float speed = 800;
	// Use this for initialization
	void Start ()
	{
	    speed /= 100;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position += transform.forward*speed*Time.deltaTime;
	}

    void LateUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit,
            (transform.forward*speed*Time.deltaTime).magnitude))
        {
            OnHit(hit.transform.gameObject);
        }
    }

    void OnHit(GameObject other)
    {
        Debug.Log("bang");
        Actor hitActor = other.GetComponent<Actor>();
        if (hitActor)
        {
            hitActor.Die();
            Destroy(gameObject);
            return;
        }

        TurretNode hitTurretNode = other.GetComponent<TurretNode>();
        if (!hitTurretNode)
        {
            Destroy(Instantiate(Resources.Load("BulletSplash"), transform.position, Quaternion.Euler(0, -180, -180)), 0.5f);
            Destroy(gameObject);
        }
    }
}
