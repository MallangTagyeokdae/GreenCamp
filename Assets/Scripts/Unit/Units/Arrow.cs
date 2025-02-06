using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private GameObject _tartget;
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
        if(other.gameObject == _tartget){
            _tartget.GetComponent<PhotonView>().RPC("AttackRequest", RpcTarget.MasterClient, _unit.unitPower);
            StopCoroutine(coroutine);
            gameObject.SetActive(false);
        }
    }

    public IEnumerator Shoot(){

        yield return null;
    }

    public void SetTarget(GameObject target){
        _tartget = target;
    }

    public void SetUnit(Unit unit){
        _unit = unit;
    }

    private IEnumerator Launch(){
        UnityEngine.Vector3 currentPos;
        UnityEngine.Vector3 targetPos;
        for(float time = 0f; time < 0.5f; time += Time.deltaTime){
            currentPos = gameObject.transform.position;
            targetPos = _tartget.transform.position;
            float distance = UnityEngine.Vector3.Distance(currentPos, targetPos);
            UnityEngine.Vector3 dir = (targetPos - currentPos).normalized;
            transform.position += dir * distance * time/0.5f;

            yield return null;
        }

        _tartget.GetComponent<PhotonView>().RPC("AttackRequest", RpcTarget.MasterClient, _unit.unitPower);
        StopCoroutine(coroutine);
        gameObject.SetActive(false);
    }

    public void LaunchArrow(){
        coroutine = StartCoroutine(Launch());
    }
}
