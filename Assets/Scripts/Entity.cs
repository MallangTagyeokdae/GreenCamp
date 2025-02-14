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
    public int armor { get; set; }
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    public int population { get; set; }
    public int fow { get; set; } // 시야
    public int fowIndex { get; set; } // 시야 정보를 담은 List에서 순서 저장
    public Slider healthBar;
    public GameObject clickedEffect;
    public GameObject enemyClickedEffect;
    //private CancellationTokenSource end = new CancellationTokenSource();
    Coroutine end;

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
    public void AttackRequest(int damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentHealth -= damage;
            // MasterClient에서 체력을 계산
            this.GetComponent<PhotonView>().RPC("SyncAttack", RpcTarget.All, currentHealth); // 계산한 체력을 넘겨줘서 동기화시킴
        }
    }

    [PunRPC]
    public void SyncAttack(float health)
    {
        if(end != null){
            StopCoroutine(end);
        }

        currentHealth = health;

        healthBar.value = currentHealth/maxHealth;
        end = StartCoroutine(ActiveHealthBar());

        GameManager.instance.UpdateEventUI(gameObject);

        if(currentHealth <= 0 && CheckIsDie(gameObject))
        {
            GameManager.instance.DestroyEntity(gameObject);
        }

    }
    private IEnumerator ActiveHealthBar(){
        float time;
        GameObject parent = healthBar.gameObject.transform.parent.gameObject;
        if (!healthBar.gameObject.activeSelf)
        {
            healthBar.gameObject.SetActive(true);
        }
        //Debug.Log("실행이 아예 안되나?");
        for (time = 0f; time < 3; time += Time.deltaTime)
        {
            // Debug.Log($"heath bar check: {time}");
            yield return null;
        }
        healthBar.gameObject.SetActive(false);
    }
    
    public virtual void DestroyEntity() {}

    private bool CheckIsDie(GameObject entity)
    {
        if(entity.TryGetComponent(out Unit unit))
        {
            if(unit.state == Unit.State.Die)
            {
                return false;
            }
        } else if(entity.TryGetComponent(out Building building))
        {
            if(building.state == Building.State.Destroy)
            {
                return false;
            }
        }
        return true;
    }

    /*public async Task ActiveHealthBarAsync(CancellationTokenSource end)
    { 
        float time;
        GameObject parent = healthBar.gameObject.transform.parent.gameObject;
        Debug.Log($"parent name: {healthBar.gameObject}");
        if (healthBar.gameObject.activeSelf)
        {
            healthBar.gameObject.SetActive(true);
        }
        Debug.Log("실행이 아예 안되나?");
        for (time = 0f; time < 3; time += Time.deltaTime)
        {
            Debug.Log($"heath bar check: {time}");
            await Task.Yield();
        }
        healthBar.gameObject.SetActive(false);
    }*/
}
