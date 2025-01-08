using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerRangeHandler : MonoBehaviour
{
    private Unit unit;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        unit = gameObject.GetComponentInParent<Unit>();
        animator = unit.GetComponent<Animator>();
    }
    private void OnTriggerStay(Collider other)
    {
        Unit enemyUnit = other.GetComponent<Unit>();
        if (enemyUnit != null && enemyUnit.teamID != unit.teamID)
        {
            Debug.Log("적군 감지");
            animator.SetTrigger("isAttacking");
        }
        // if (enemyUnit != null && enemyUnit.teamID == unit.teamID)
        // {
        //     animator.SetTrigger("isAttacking");
        // }
    }
    private void OnTriggerExit(Collider other)
    {
        animator.ResetTrigger("isAttacking");
    }
}
