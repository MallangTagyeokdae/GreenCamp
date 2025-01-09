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
    [HideInInspector] public UnityEvent<GameObject> attTrigger;
    [HideInInspector] public UnityEvent<GameObject> aggTrigger;
    public void OnChildTriggerEnter(GameObject go, CollisionRange coll)
    {
        switch (coll)
        {
            case CollisionRange.attack:
                attTrigger?.Invoke(go);
                break;
            case CollisionRange.aggretion:
                aggTrigger?.Invoke(go);
                break;
        }
    }
}
