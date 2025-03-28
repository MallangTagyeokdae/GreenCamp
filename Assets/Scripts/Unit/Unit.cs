using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FischlWorks_FogWar;
using Photon.Pun;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public abstract class Unit : Entity
{
    public enum Order // 유닛에게 내려진 유저의 명령
    {
        Idle = 0,
        Move = 1,

        Offensive = 2,
        Attack = 3
    }

    public enum State //현재 실행 중인 유닛의 동작
    {
        Idle,
        Move,
        Aggregated,
        Attack,
        Die
    }


    //public string teamID; //{ get; set; }
    public int unitID { get; set; }
    public string unitType { get; set; }
    public Vector3 unitLocation { get; set; }
    public Vector3 destination { get; set; }
    public int unitCost { get; set; }
    public int unitPower { get; set; }
    public int unitPowerRange { get; set; }
    public int unitMoveSpeed { get; set; }
    public int populationCost { get; set; }

    public bool isTurretUnit = false;
    private Quaternion _initRotation;
    public State state = State.Idle;
    public GameObject target;
    public GameObject projectileTarget;

    [HideInInspector] public Animator animator;
    private new Rigidbody rigidbody;
    public Coroutine unitBehaviour;
    public GameObject prefab;
    public Transform prefabPosition;

    public Order order = Order.Idle;

    private void Awake()
    {
        if (attTriggerEnter == null)
        {
            attTriggerEnter = new UnityEvent<GameObject>();
        }
        if (attTriggerExit == null)
        {
            attTriggerExit = new UnityEvent<GameObject>();
        }
        if (aggTriggerEnter == null)
        {
            aggTriggerEnter = new UnityEvent<GameObject>();
        }
        if (aggTriggerExit == null)
        {
            aggTriggerExit = new UnityEvent<GameObject>();
        }
        attackList = new HashSet<GameObject>();
        aggList = new HashSet<GameObject>();
        animator = GetComponent<Animator>();
        _initRotation = gameObject.transform.rotation;
        if (!isTurretUnit)
        {
            healthBar = gameObject.transform.Find("HealthBar/UnitCurrentHealth").GetComponent<Slider>();
            healthBar.gameObject.SetActive(false);
            rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            clickedEffect = transform.Find("ClickedEffect").gameObject;
            enemyClickedEffect = transform.Find("EnemyClickedEffect").gameObject;
            if (!gameObject.GetComponent<PhotonView>().IsMine)
            {
                gameObject.GetComponent<ClickEventHandler>().rightClickDownEvent.AddListener((Vector3 pos) =>
                    {
                        GameManager.instance.SetTargetObject(gameObject, 1);
                    }
                );
                gameObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) =>
                    {
                        GameManager.instance.SetTargetObject(gameObject, 0);
                    }
                );
            }
        }
    }

    private void Start()
    {

    }

    public Unit(string teamID,
                int unitID,
                string unitType,
                Vector3 unitLocation,
                int unitMaxHealth,
                int unitCurrentHealth,
                int unitCost,
                int unitPower,
                int unitPowerRange,
                int unitMoveSpeed,
                int populationCost)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = unitType;
        transform.position = unitLocation;
        this.maxHealth = unitMaxHealth;
        this.currentHealth = unitCurrentHealth;
        this.unitCost = unitCost;
        this.unitPower = unitPower;
        this.unitPowerRange = unitPowerRange;
        this.unitMoveSpeed = unitMoveSpeed;
        this.populationCost = populationCost;
    }

    public void SetAttEnter(Action<GameObject> action)
    {
        attTriggerEnter.AddListener((GameObject go) => action(go));
    }
    public void SetAttExit(Action<GameObject> action)
    {
        attTriggerExit.AddListener((GameObject go) => action(go));
    }
    public void SetAggEnter(Action<GameObject> action)
    {
        aggTriggerEnter.AddListener((GameObject go) => action(go));
    }
    public void SetAggExit(Action<GameObject> action)
    {
        aggTriggerExit.AddListener((GameObject go) => action(go));
    }

    public virtual void OnIdleEnter()
    { //attackList와 aggList 내의 null 값들을 전부 제거해준 후 attack또는 aggro 함수를 실행해준다.

        foreach (GameObject enemy in attackList.ToList())
        {
            if (enemy == null || enemy.CompareTag("Untagged"))
            {
                attackList.Remove(enemy);
            }
        }

        foreach (GameObject enemy in aggList.ToList())
        {
            if (enemy == null || enemy.CompareTag("Untagged"))
            {
                aggList.Remove(enemy);
            }
        }

        if (attackList.Count != 0)
        {
            //콜백함수 실행 후 리턴
            GameManager.instance.AttackUnit(this.gameObject, attackList.ToList<GameObject>()[0]); // 일단 하드 코딩함 근데 콜백 따로 만들어서 하는 것보단 이게 나을 수도?
            return;
        }

        if (aggList.Count != 0)
        {
            //콜백함수 실행 후 리턴
            GameManager.instance.Aggregated(this.gameObject, aggList.ToList<GameObject>()[0]); // 일단 하드 코딩함 근데 콜백 따로 만들어서 하는 것보단 이게 나을 수도?
            return;
        }

        if(destination != Vector3.zero && Vector3.Distance(destination, gameObject.transform.position) >= 0.1f){
            target = null;

            /*if (unitBehaviour != null)
            {
                StopCoroutine(unitBehaviour);
            }
            unitBehaviour = StartCoroutine(GameManager.instance.unitController.Move(gameObject, destination, 3));*/
            GameManager.instance.Attang(destination, 3, gameObject);
        }

        else
        {
            destination = Vector3.zero;
        }

    }

    public void SetOrder(int index)
    {
        switch (index)
        {
            case 0:
                order = Order.Idle;
                break;

            case 1:
                order = Order.Move;
                break;

            case 2:
                order = Order.Offensive;
                break;

            case 3:
                order = Order.Attack;
                break;
        }
    }


    public void SetState(string newState)
    {
        switch (newState)
        {
            case "Idle":
                state = State.Idle;
                if (!isTurretUnit)
                {
                    rigidbody.isKinematic = true;
                }
                else
                {
                    transform.rotation = _initRotation;
                }
                OnIdleEnter();
                break;

            case "Move":
                state = State.Move;
                rigidbody.isKinematic = false;
                if (gameObject.GetComponent<PhotonView>().IsMine)
                {
                    animator.SetBool("isWalking", true);
                }
                break;
            case "Aggregated":
                state = State.Aggregated;
                rigidbody.isKinematic = false;
                if (gameObject.GetComponent<PhotonView>().IsMine)
                {
                    animator.SetBool("isWalking", true);
                }
                break;

            case "Attack":
                state = State.Attack;
                if (!isTurretUnit)
                {
                    rigidbody.isKinematic = true;
                }
                if (gameObject.GetComponent<PhotonView>().IsMine)
                {
                    if (gameObject.TryGetComponent(out Healer healer))
                    {
                        Debug.Log("이게 실행이 안될리가 없는데?");
                        if (healer.isCool == false)
                        {
                            Debug.Log("이게 실행이 안될리가 없는데?2");

                            // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                            // if ( !animator.IsInTransition(0) && stateInfo.normalizedTime < 0.1f)
                            // {
                            // Debug.Log("애니메이션 실행 안 하는 중");

                            animator.SetBool("isAttacking", true);
                            // animator.Play("staff_07_cast_B", 0, 0f);
                            // } 
                        }
                    }
                    else
                    {
                        animator.SetBool("isAttacking", true);
                    }
                }
                break;
            case "Die":
                state = State.Die;
                foreach (Collider collider in GetComponents<Collider>())
                {
                    collider.enabled = false;
                }
                break;
        }
    }

    [PunRPC]
    public void ChangeState(string newState)
    {
        switch (state)
        {
            case State.Idle:
                if (newState == "Idle")
                {
                    break;
                }
                SetState(newState);
                break;

            case State.Move:
                if (newState == "Move")
                {
                    break;
                }
                if (gameObject.GetComponent<PhotonView>().IsMine)
                {
                    animator.SetBool("isWalking", false);
                }
                SetState(newState);
                break;
            
            case State.Aggregated:
                if (newState == "Aggregated")
                {
                    break;
                }
                if (gameObject.GetComponent<PhotonView>().IsMine)
                {
                    animator.SetBool("isWalking", false);
                }
                SetState(newState);
                break;

            case State.Attack:
                if (newState == "Attack")
                {
                    break;
                }
                if (gameObject.GetComponent<PhotonView>().IsMine)
                {
                    animator.SetBool("isAttacking", false);
                }
                SetState(newState);
                break;

            case State.Die:
                break;
        }
    }
    //새로운 객체를 찾아야 하는 경우 -> 유닛의 상태가 idle이 되었을 때(이동을 완료했을 때, 공격하던 객체가 죽었을 때, 공격하던 객체가 어그로 범위를 벗어났을 때)
    public override void DestroyEntity()
    {
        gameObject.GetComponent<PhotonView>().RPC("SyncSetTag", RpcTarget.AllBuffered, "Untagged");
        gameObject.GetComponent<PhotonView>().RPC("ChangeState", RpcTarget.AllBuffered, "Die");
        GameStatus.instance.currentUnitCount -= population;
        animator.SetTrigger("isDead");
    }
    public void CheckDie()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void SyncSetTag(string tag)
    {
        gameObject.tag = tag;
    }

    public void AttackReq()
    {
        if (gameObject.GetComponent<PhotonView>().IsMine)
        {
            if(target != null)
            {
                target.GetComponent<PhotonView>().RPC("AttackRequest", RpcTarget.MasterClient, unitPower);
            }
            else
            {
                Debug.Log("타겟 널이었음");
            }
        }
    }

    public void HealReq()
    {
        if (gameObject.GetComponent<PhotonView>().IsMine)
        {
            foreach (GameObject ally in attackList)
            {
                // Debug.Log("HealReq 테스트: " + ally.name);
                ally.GetComponent<PhotonView>().RPC("HealRequest", RpcTarget.MasterClient, unitPower);
            }
            gameObject.GetComponent<Healer>().isCool = true;
            gameObject.GetComponent<Healer>().CoolTime();
        }
    }
    public void HealEffect()
    {
        if (gameObject.GetComponent<Healer>())
        {
            if (gameObject.GetComponent<Healer>().HealingEffect.isPlaying)
            {
                gameObject.GetComponent<Healer>().HealingEffect.Stop();
            }
            gameObject.GetComponent<Healer>().HealingEffect.Play();
        }
    }


    [PunRPC]
    public void SetTarget(int viewID)
    {
        projectileTarget = PhotonView.Find(viewID).gameObject;
    }

    public void Launch()
    {
        if (projectileTarget != null)
        {
            GameObject pjtObject = Instantiate(prefab, prefabPosition.position, Quaternion.identity);
            pjtObject.AddComponent<Projectile>();
            pjtObject.TryGetComponent(out Projectile projectile);
            projectile.SetArrowTarget(projectileTarget);
            projectile.SetUnit(this);
            projectile.LaunchProjectile();
        }
    }

}
