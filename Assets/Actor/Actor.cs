using UnityEngine;
using Random = System.Random;

public abstract class Actor : MonoBehaviour
{
    protected Random r = new Random();
    protected Vector3 heading = Vector3.right;
    protected Grid WorldGrid;
    public abstract void OnAdd(Grid inGrid);
    public abstract void OnRemove();
    public abstract void Die();
    public abstract void Win();
}
