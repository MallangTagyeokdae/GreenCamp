using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TurretMagician : Unit
{
    public TurretMagician(string teamID, int unitID, Vector3 unitLocation)

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

    public GameObject fireBall_prefab;
    public Transform fireBallPosition;

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
        this.unitType = "Magician";
        this.unitLocation = unitLocation;
        this.unitCost = 40;
        this.unitPowerRange = 14;
        this.unitMoveSpeed = 5;
        this.populationCost = 1;
        this.population = 1;
        this.fow = 30;
    }

    public void Launch(){
        if(target != null){
            GameObject fireball = Instantiate(fireBall_prefab, fireBallPosition.position, Quaternion.identity);
            fireball.AddComponent<Projectile>();
            fireball.TryGetComponent(out Projectile projectile);
            projectile.SetArrowTarget(target);
            projectile.SetUnit(this);
            projectile.LaunchProjectile();
        }
    }
}
