using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    public int population { get; set; }
    public Slider healthBar;
    
    public GameObject clickedEffect;
    private CancellationTokenSource end = new CancellationTokenSource();

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


    [PunRPC]
    public void AttackRequest()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.GetComponent<PhotonView>().RPC("SyncAttack", RpcTarget.All);
        }
    }

    [PunRPC]
    public void SyncAttack()
    {
        end.Cancel();
        currentHealth -= 10;
        healthBar.value = currentHealth/maxHealth;
        Task.Run(() => ActiveHealthBar(end));
    }

    private void ActiveHealthBar(CancellationTokenSource end)
    {
        float time;
        GameObject parent = healthBar.gameObject.transform.parent.gameObject;
        if (parent.activeSelf)
        {
            parent.SetActive(true);
        }
        for (time = 0f; time < 3; time += Time.deltaTime)
        {
        }
        parent.SetActive(false);
    }
}
