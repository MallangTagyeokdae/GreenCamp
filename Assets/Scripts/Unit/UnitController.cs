using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public List<Unit> unitList;
    private int _unitCount;
    public void Initialize()
    {
        _unitCount = 0;
    }
    public void createUnit(Unit unit)
    {
        unitList.Add(unit);
    }
    public void unitAttacked(int unitID, int damage)
    {
        Unit selectedUnit = unitList[unitID];
    }

    public void destroyUnit(int unitID)
    {

    }
    public void moveUnit(int unitID)
    {
        
    }
}
