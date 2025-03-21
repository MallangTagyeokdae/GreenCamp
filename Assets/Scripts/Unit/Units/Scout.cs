using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Unit을 상속받는 자식 Archer Class
public class Scout : Unit
{
    void Start()
    {
        this.maxHealth = 50 + GameStatus.instance.healthIncrease;
        this.currentHealth = 50 + GameStatus.instance.healthIncrease;
        this.armor = 1 + GameStatus.instance.armorIncrease;
        // this.unitPower = 0 + GameStatus.instance.damageIncrease;
    }
    // teamID, unitID, unitLocation은 생성자 호출 시 전달된 값으로 초기화됨 (생성자 매개변수로 전달되어 부모 클래스에서 초기화됨)

    public Scout(string teamID, int unitID, Vector3 unitLocation)

                // teamID, unitID, unitLocation 외의 다른 것들은 기본값으로 설정됨
                : base(
                teamID,
                unitID,
                unitType: "Scout",
                unitLocation,
                unitMaxHealth: 50,
                unitCurrentHealth: 50,
                unitCost: 20,
                unitPower: 0,
                unitPowerRange: 0,
                unitMoveSpeed: 20,
                populationCost: 1)
    {
        // 추가 초기화 작업 수행 가능
    }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Scout";
        this.unitLocation = unitLocation;
        this.unitCost = 40;
        this.unitPowerRange = 0;
        this.unitMoveSpeed = 9;
        this.populationCost = 1;
        this.population = 1;
        this.fow = 40;
    }
}
