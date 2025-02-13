using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private GameObject _target;
    private Unit _unit;
    private Coroutine coroutine;
    private UnityEngine.Vector3 initPos;


    private void Awake(){
        initPos = transform.localPosition;
    }

    private void OnDisable() {
        transform.position = transform.parent.TransformPoint(initPos);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == _target){
            if(gameObject.transform.parent.gameObject.GetComponent<PhotonView>().IsMine == false){  //화살을 쏜 유닛이 자신의 유닛이 아닌 경우 return
                StopCoroutine(coroutine);
                gameObject.SetActive(false);
                return;
            }
            else{
                if(_target != null){
                    _target.GetComponent<PhotonView>().RPC("AttackRequest", RpcTarget.MasterClient, _unit.unitPower);  //화살을 쏜 유닛이 자신의 유닛인 경우 적에게 맞았을 때 마스터 클라이언트에게 판정을 요구
                }
                StopCoroutine(coroutine);
                gameObject.SetActive(false);
            }        
        }
    }

    public void SetArrowTarget(GameObject target){
        _target = target;
    }

    public void SetUnit(Unit unit){
        _unit = unit;
    }

    private IEnumerator Launch(){
        UnityEngine.Vector3 currentPos = UnityEngine.Vector3.zero;;
        UnityEngine.Vector3 targetPos;
        for(float time = 0f; time < 1f; time += Time.deltaTime){
            if(_target != null){
                currentPos = gameObject.transform.position;
                targetPos = _target.transform.position + new UnityEngine.Vector3(0, 1.2f, 0);
                float distance = UnityEngine.Vector3.Distance(currentPos, targetPos);
                UnityEngine.Vector3 dir = (targetPos - currentPos).normalized;
                transform.position += dir * distance * time/1f;

                yield return null;
            }
        }

        if(_target != null){
            _target.GetComponent<PhotonView>().RPC("AttackRequest", RpcTarget.MasterClient, _unit.unitPower);
        }
        gameObject.SetActive(false);
    }

    public void LaunchArrow(){
        coroutine = StartCoroutine(Launch());
    }
}
