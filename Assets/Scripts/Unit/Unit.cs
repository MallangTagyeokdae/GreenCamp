using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;
using UnityEngine.Events;
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
        Attack
    }


    //public string teamID; //{ get; set; }
    public int unitID { get; set; }
    public string unitType { get; set; }
    public Vector3 unitLocation { get; set; }
    public int unitMaxHealth { get; set; }
    public int unitCurrentHealth { get; set; }
    public int unitCost { get; set; }
    public int unitPower { get; set; }
    public int unitPowerRange { get; set; }
    // private Collider u
    public int unitMoveSpeed { get; set; }
    public int populationCost { get; set; }
    public Slider healthBar;
    public State state = State.Idle;
    

    [HideInInspector] public Animator animator;

    public Coroutine unitBehaviour;

    public Order order = Order.Idle;

    private void Awake()
    {
        if (attTrigger == null)
        {
            attTrigger = new UnityEvent<GameObject>();
        }
        attackList = new HashSet<GameObject>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        healthBar = gameObject.transform.Find("HealthBar/UnitCurrentHealth").GetComponent<Slider>();
        healthBar.gameObject.SetActive(false);
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
        this.unitMaxHealth = unitMaxHealth;
        this.unitCurrentHealth = unitCurrentHealth;
        this.unitCost = unitCost;
        this.unitPower = unitPower;
        this.unitPowerRange = unitPowerRange;
        this.unitMoveSpeed = unitMoveSpeed;
        this.populationCost = populationCost;
    }

    public void SetAttCollision(Action<GameObject> action)
    {
        attTrigger.AddListener((GameObject go) => action(go));
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

    public void SetAnimation(string newState)
    {
        switch (newState)
        {
            case "Idle":
                state = State.Idle;
                break;

            case "Move":
                state = State.Move;
                animator.SetBool("isWalking", true);
                break;

            case "Attack":
                state = State.Attack;
                animator.SetBool("isAttacking", true);
                break;
        }
    }

    public void ChangeAnimation(string newState)
    {
        switch (state)
        {
            case State.Idle:
                if (newState == "Idle")
                {
                    break;
                }
                SetAnimation(newState);
                break;

            case State.Move:
                if (newState == "Move")
                {
                    break;
                }
                animator.SetBool("isWalking", false);
                SetAnimation(newState);
                break;

            case State.Attack:
                if (newState == "Attack")
                {
                    break;
                }
                animator.SetBool("isAttacking", false);
                SetAnimation(newState);
                break;
        }

    }
}
