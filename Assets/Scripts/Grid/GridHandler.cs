using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    public GameObject grid;
    public List<Collider> detectedGrids;
    public List<Collider> constructionGrids;

    private float _buildingRange;
    private Vector3 _range;
    private Vector3 areaPos;
    private LayerMask _layerMask = 1 << 3;

    public void HoveredGrid(Vector3 hitPoint)
    {
        if(grid.activeSelf)
        {
            _range = new Vector3(_buildingRange,1,_buildingRange);
            areaPos = new Vector3(hitPoint.x, 0.01f, hitPoint.z);
            
            // 넓은 범위
            Collider[] detectedObjs = Physics.OverlapSphere(new Vector3(areaPos.x, 0.02f, areaPos.z),18.5f,_layerMask);
            // 건물이 지어지는 Grid만
            Collider[] constructionGrid = Physics.OverlapBox(new Vector3(areaPos.x, 0.02f, areaPos.z),_range*2,Quaternion.identity,_layerMask);
            UpdateGridMeshToDefault(detectedObjs);
            // UpdateGridMeshToDefault(constructionGrid);
            // 기존 리스트에서 빠져나갈 요소들의 Mesh값을 바꿔줬으니 새로 받은 Grid 배열을 List로 추가해줌
            detectedGrids = detectedObjs.ToList();
            constructionGrids = constructionGrid.ToList();
            // 최신화된 Grid List의 요소들을 검사해서 Builted && Hovered 상태가 아니면 Mesh를 바꿔준다.
            UpdateGridMeshToHovered();
            UpdateGridMeshToSelected();
           
        }
    }

    public void SetBuildingRange(float range)
    {
        _buildingRange = range;
    }
    private void UpdateGridMeshToDefault(Collider[] detectedObjs)
    {
        // 기존 Grid List 요소를 검사해서 새로 감지된 Grid 배열에 요소가 없다면
        // 마우스의 범위에 벗어난것이므로 요소의 Mesh값을 Default값으로 바꾼다.
        // 이때 Builted 상태인 Grid는 Mesh값 업데이트 X
        if(detectedGrids.Count != 0)
        {
            foreach(Collider obj in detectedGrids)
            {
                if(obj.TryGetComponent(out GridEvent grid))
                {
                    if(!detectedObjs.Contains(obj) && !grid.GetIsBuilted() && !grid.GetIsHasObject())
                    {
                        grid.SetDefault();
                        grid.ChangeMesh();
                    }
                    grid.UnSetRender();
                }
            }
        }
    }

    private void UpdateGridMeshToHovered()
    {
        foreach(Collider obj in detectedGrids)
        {
            if(obj.TryGetComponent(out GridEvent grid))
            {
                grid.CheckHasObject();
                if(!grid.GetIsBuilted() && !grid.GetIsHasObject())
                {
                    grid.SetHovered();
                    grid.ChangeMesh();
                }
                grid.SetRander();
            }
        }
    }

    private void UpdateGridMeshToSelected()
    {
        foreach(Collider obj in constructionGrids)
        {
            if(obj.TryGetComponent(out GridEvent grid))
            {
                if(!grid.GetIsBuilted() && !grid.GetIsHasObject())
                {
                    grid.SetSelected();
                    grid.ChangeMesh();
                }
            }
        }
    }

    public void SetGridsToBuilted()
    {
        foreach(Collider obj in constructionGrids)
        {
            if(obj.TryGetComponent(out GridEvent grid))
            {
                grid.SetBuilted();
                grid.ChangeMesh();
            }
        }
    }

    public Vector3 CalculateGridScalse()
    {
        Vector3 center = Vector3.zero;

        foreach(Collider collider in constructionGrids)
        {
            center += collider.gameObject.transform.position;
        }

        return center / constructionGrids.Count();
    }

    public bool CheckCanBuilt()
    {
        foreach(Collider obj in constructionGrids)
        {
            if(obj.TryGetComponent(out GridEvent grid))
            {
                if(grid.GetIsBuilted() || grid.GetIsHasObject()) return false;
            }
        }
        return true;
    }
}
