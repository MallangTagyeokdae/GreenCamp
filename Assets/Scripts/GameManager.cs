using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> gameStates;
    public UIController uIController;
    private BuildingController _buildingController;
    
    public Building selectedBuilding;
    public void Update()
    {
        // -------------------------------
        // Building 관련 로직
        if(GameStatus.instance.canCreateBuilding(selectedBuilding))
        { // 건물을 생성할 수 있음
            _buildingController.createBuilding(selectedBuilding);
        }
        // -------------------------------
    }
        
       

}
