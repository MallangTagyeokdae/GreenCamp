using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Unit을 상속받는 자식 Archer Class
public class Archer : Unit
{
    void Start()
    {
        this.maxHealth = 50;
        this.currentHealth = 50;
        this.unitPower = 35;
    }
    public GameObject arrow;
    // teamID, unitID, unitLocation은 생성자 호출 시 전달된 값으로 초기화됨 (생성자 매개변수로 전달되어 부모 클래스에서 초기화됨)

    public Archer(string teamID, int unitID, Vector3 unitLocation)

                // teamID, unitID, unitLocation 외의 다른 것들은 기본값으로 설정됨
                : base(
                teamID,
                unitID,
                unitType: "Archer",
                unitLocation,
                unitMaxHealth: 50,
                unitCurrentHealth: 50,
                unitCost: 40,
                unitPower: 35,
                unitPowerRange: 14,
                unitMoveSpeed: 3,
                populationCost: 1)
    {
        // 추가 초기화 작업 수행 가능
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

    public void Shoot(){
        //PhotonView targetView = PhotonView.Find(viewID);
        if(target != null){
            arrow.SetActive(true);
            arrow.TryGetComponent(out Arrow arr);
            arr.SetArrowTarget(target);
            arr.SetUnit(this);
            arr.LaunchArrow();
        }
    }
}
