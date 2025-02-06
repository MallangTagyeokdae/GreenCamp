using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : Entity
{
    public enum State
    {
        InCreating = 0,  // 건물이 건설 중인 상태
        Built = 1,   // 건물이 완료된 상태
        InProgress = 2,  // 건물이 진행 중인 상태
        Destroy = 3
    }

    public enum InProgressItem
    {
        None = 0,
        LevelUP = 1,
        Soldier = 2,
        Archer = 3,
        Tanker = 4,
        Healer = 5
    }

    //public string teamID { get; set; }
    public int ID { get; set; }
    public string type { get; set; }
    public Vector3 location { get; set; }
    public float progress { get; set; }
    public int cost { get; set; }
    public int levelUpCost { get; set; }
    public int level { get; set; }
    public int returnCost { get; set; }
    public int returnPopulation { get; set; }
    public Slider progressBar;
    public float time { get; set; }
    public float loadingTime { get; set; }
    public float tempSaveHealth { get; set; }
    public List<Collider> underGrid { get; set; }
    public State state = State.InCreating;
    public InProgressItem inProgressItem = InProgressItem.None;
    public Mesh progressMesh1;
    public Mesh progressMesh2;
    public Mesh completeMesh;
    public GameObject destroyEffect;
    public GameObject completeEffect;
    public GameObject levelUpEffect;
    public ClickEventHandler clickEventHandler;

    public Building(string teamID, int ID, string type, Vector3 location,
    int maxHealth, int cost, int level)
    {
        this.teamID = teamID;
        this.ID = ID;
        this.type = type;
        this.location = location;
        this.maxHealth = maxHealth;
        this.cost = cost;
        this.level = level;
        this.time = 0f;
        this.loadingTime = 10f;
    }

    private void Awake()
    {
        clickedEffect = transform.Find("ClickedEffect").gameObject;
        tempSaveHealth = 0;

        enemyClickedEffect = transform.Find("EnemyClickedEffect").gameObject;
        clickEventHandler = gameObject.GetComponent<ClickEventHandler>();
        clickEventHandler.rightClickDownEvent.AddListener((Vector3 pos) =>
        {
            GameManager.instance.SetTargetObject(gameObject);
        }
        );

        Debug.Log($"{gameObject.name} - Owner: {gameObject.GetComponent<PhotonView>().Owner}, IsMine: {gameObject.GetComponent<PhotonView>().IsMine}");
        //end = new CancellationTokenSource();
    }

    public virtual void InitTime()
    {
        time = 0f;
        loadingTime = 30 / 10f;
        //gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
        gameObject.GetComponent<PhotonView>().RPC("SetProgressMesh1", RpcTarget.AllBuffered);
    }

    public virtual void UpdateCreateBuildingTime(float update)
    {
        float incrementPerSec = maxHealth / loadingTime;
        time = update;
        tempSaveHealth += incrementPerSec * Time.deltaTime;
        progress = time / loadingTime * 100;
        if(tempSaveHealth - currentHealth > 1f)
        {
            Debug.Log("업데이트될 체력 : " + tempSaveHealth + " / 현재 체력 : " + currentHealth);
            gameObject.GetComponent<PhotonView>().RPC("tlqkftoRldi", RpcTarget.AllBuffered,tempSaveHealth, progress, state.ToString());
            
            // currentHealth = tempSaveHealth;
            // healthBar.value = (float)(currentHealth * 1.0 / maxHealth);
            // progressBar.value = (float)this.progress / 100;
        }
        UpdateMesh();
    }

    public virtual void UpdateMesh() //
    {
        if (time > loadingTime / 2 && time < loadingTime)
        {
            //this.gameObject.GetComponent<MeshFilter>().mesh = progressMesh2;
            gameObject.GetComponent<PhotonView>().RPC("SetProgressMesh2", RpcTarget.AllBuffered);
        }
        else if (time > loadingTime)
        {
            //this.gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
            gameObject.GetComponent<PhotonView>().RPC("SetCompleteMesh", RpcTarget.AllBuffered);
            gameObject.GetComponent<PhotonView>().RPC("GenerateCompleteEffect", RpcTarget.All);
        }
    }

    public virtual void InitOrderTime(float totalTime)//
    {
        state = State.InProgress;
        time = 0f;
        loadingTime = totalTime;
    }

    public virtual void UpdateOrderTime(float update)//
    {
        time = update;
        progress = time / loadingTime * 100;
        progressBar.value = (float)progress / 100;
    }

    [PunRPC]
    public virtual void SetProgressMesh1()
    {
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    [PunRPC]
    public virtual void SetProgressMesh2()
    {
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh2;
    }
    [PunRPC]
    public virtual void SetCompleteMesh()
    {
        gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
    }

    [PunRPC]
    public virtual void GenerateCompleteEffect()
    {
        completeEffect.SetActive(true);
    }

    [PunRPC]
    public virtual void ActiveLevelUpEffect(bool active = true)
    {
        levelUpEffect.SetActive(active);
    }

    [PunRPC]
    public virtual void ActiveDestroyEffect()
    {
        destroyEffect.SetActive(true);
    }

    [PunRPC]
    public void SyncBuildingHealth(float health, float progress, string state)
    {
        Enum.TryParse(state, out State currentState);

        currentHealth = health;
        this.progress = progress;
        this.state = currentState;

        Debug.Log("바뀐 체력 : " + currentHealth + " / 현재 진행률 : " + progress);

        healthBar.value = (float)(currentHealth * 1.0 / maxHealth);
        progressBar.value = (float)this.progress / 100;
    }

}
