using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CollisionRange
{
    attack,
    aggretion
}


public class Entity : MonoBehaviour
{
    [HideInInspector] public UnityEvent<GameObject> attTriggerEnter;
    [HideInInspector] public UnityEvent<GameObject> attTriggerExit;
    [HideInInspector] public UnityEvent<GameObject> aggTriggerEnter;
    [HideInInspector] public UnityEvent<GameObject> aggTriggerExit;
    public HashSet<GameObject> attackList;
    public HashSet<GameObject> aggList;
    public string teamID;

    public void OnChildTriggerEnter(GameObject go, CollisionRange coll)
    {
        switch (coll)
        {
            case CollisionRange.attack:
                attTriggerEnter?.Invoke(go);
                break;
            case CollisionRange.aggretion:
                aggTriggerEnter?.Invoke(go);
                break;
        }
    }

    public void OnChildTriggerExit(GameObject go, CollisionRange coll)
    {
        switch (coll)
        {
            case CollisionRange.attack:
                attTriggerExit?.Invoke(go);
                break;
            case CollisionRange.aggretion:
                aggTriggerExit?.Invoke(go);
                break;
        }
    }
}
