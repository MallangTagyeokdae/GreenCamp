using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unit을 상속받는 자식 Archer Class
public class Archer : Unit
{
    // teamID, unitID, unitLocation은 생성자 호출 시 전달된 값으로 초기화됨 (생성자 매개변수로 전달되어 부모 클래스에서 초기화됨)
    public Archer(string teamID, int unitID, Vector3 unitLocation)

                // teamID, unitID, unitLocation 외의 다른 것들은 기본값으로 설정됨
                : base(
                teamID,
                unitID,
                unitType: "Archer",
                unitLocation,
                unitHealth: 0,
                unitCost: 0,
                unitPower: 0,
                unitPowerRange: 0,
                unitMoveSpeed: 0,
                populationCost: 0)
    {
        // 추가 초기화 작업 수행 가능
    }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Archer";
        this.unitLocation = unitLocation;
        this.unitHealth = 0;
        this.unitCost = 0;
        this.unitPower = 0;
        this.unitPowerRange = 0;
        this.unitMoveSpeed = 3;
        this.populationCost = 0;
    }
}
