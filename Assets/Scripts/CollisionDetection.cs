using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private GameObject root;
    public CollisionRange collisionRange;

    private void Start()
    {
        root = gameObject.transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        root = gameObject.transform.parent.gameObject;
        if (root.TryGetComponent(out Entity entity) && root.GetComponent<PhotonView>().IsMine)
        {
            entity.OnChildTriggerEnter(other.gameObject, collisionRange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        root = gameObject.transform.parent.gameObject;
        if (root.TryGetComponent(out Entity entity)&& root.GetComponent<PhotonView>().IsMine)
        {
            entity.OnChildTriggerExit(other.gameObject, collisionRange);
        }
    }
}
