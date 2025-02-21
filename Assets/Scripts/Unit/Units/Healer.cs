using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Healer : Unit
{
    void Start()
    {
        this.maxHealth = 85 + GameStatus.instance.healthIncrease;
        this.currentHealth = 85 + GameStatus.instance.healthIncrease;
        this.unitPower = 10 + GameStatus.instance.damageIncrease;
    }
    public ParticleSystem HealingEffect;
    public bool isCool = false;
    public Healer(string teamID, int unitID, Vector3 unitLocation)
                : base(
                teamID,
                unitID,
                unitType: "Healer",
                unitLocation,
                unitMaxHealth: 85,
                unitCurrentHealth: 85,
                unitCost: 65,
                unitPower: 10,
                unitPowerRange: 4,
                unitMoveSpeed: 1,
                populationCost: 1)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Healer";
        this.unitLocation = unitLocation;
        this.unitCost = 65;
        this.unitPowerRange = 4;
        this.unitMoveSpeed = 3;
        this.populationCost = 1;
        this.population = 2;
        this.fow = 30;
        this.armor = 5 + GameStatus.instance.armorIncrease;
    }
    public override void OnIdleEnter()
    {
        Debug.Log("이거 왜 안되지? IdleEnter Check");
        // base.OnIdleEnter();
        foreach (GameObject ally in attackList.ToList())
        {
            if (ally == null || ally.CompareTag("Untagged"))
            {
                attackList.Remove(ally);
            }

        }
        if (attackList.Count != 0)
        {
            //콜백함수 실행 후 리턴
            /*foreach (GameObject ally in attackList.ToList())
            {
                Debug.Log("콜백함수 실행: " + ally.name);
                
                break;
            }*/
            Debug.Log("IdleEnter Check");
            GameManager.instance.Heal(gameObject);
        }
    }
    public async Task CoolTime(float coolTime)
    {
        float realTime = 0f;
        while (realTime < coolTime)
        {
            await Task.Yield();  // 다음 프레임까지 대기
            realTime += Time.deltaTime;
        }
        isCool = false;
        if (state == State.Attack)
        {
            ChangeState("Idle");
        }
    }
}

//애니메이션 트랜잭션 시간보다 더 빠르게 코드가 전환이 되어서 animaition이 idle 상태로 갔다가 heal을 해야하는데 heal에 계속 머물러 있음.
