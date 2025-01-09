using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private GameObject root;
    public CollisionRange collisionRange;

    private void Start() {
        root = gameObject.transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other) {
        if(root.TryGetComponent(out Entity entity)){
            entity.OnChildTriggerEnter(other.gameObject, collisionRange);
        }
    }
}
