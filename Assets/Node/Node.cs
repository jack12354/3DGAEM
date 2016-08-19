using System;
using UnityEngine;
using Random = System.Random;

public abstract class Node : MonoBehaviour
{
    public bool IsPassable;
    public bool IsBuildable;
    public bool Flagged;
    public Grid Parent { get; set; }

    public abstract void OnAdd();
    public abstract void OnRemove();
    public abstract void OnEnter(Actor inActor);

    protected Random r = new Random();
}

