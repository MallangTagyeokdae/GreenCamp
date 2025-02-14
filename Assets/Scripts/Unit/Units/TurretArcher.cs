using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretArcher : Unit
{
     public TurretArcher(string teamID, int unitID, Vector3 unitLocation)

                // teamID, unitID, unitLocation 외의 다른 것들은 기본값으로 설정됨
                : base(
                teamID,
                unitID,
                unitType: "TurretArcher",
                unitLocation,
                unitMaxHealth: 1000000,
                unitCurrentHealth: 1000000,
                unitCost: 0,
                unitPower: 35,
                unitPowerRange: 14,
                unitMoveSpeed: 3,
                populationCost: 1)
    {
        // 추가 초기화 작업 수행 가능
    }

    void Start()
    {
        this.maxHealth = 1000000;
        this.currentHealth = 1000000;
        this.unitPower = 35;
        if(GameStatus.instance.teamID == teamID){
            SetAttEnter((GameObject enemy) => { GameManager.instance.AttackUnit(gameObject, enemy); });
            SetAttExit((GameObject enemy) => { attackList.Remove(enemy); });
        }
    }

    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Archer";
        this.unitLocation = unitLocation;
        this.unitCost = 40;
        this.unitPowerRange = 14;
        this.unitMoveSpeed = 5;
        this.populationCost = 1;
        this.population = 1;
        this.fow = 30;
    }
}
