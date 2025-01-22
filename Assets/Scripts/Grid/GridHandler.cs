using System;
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
    public List<GridList> startingPoints;

    private float _buildingRange;
    private Vector3 _range;
    private Vector3 areaPos;
    private LayerMask _layerMask = 1 << 3;
    
    [System.Serializable]
    public class GridList
    {
        public List<Collider> gridList = new List<Collider>();
    }

    public void HoveredGrid(Vector3 hitPoint)
    {
        if(grid.activeSelf)
        {
            _range = new Vector3(_buildingRange,1,_buildingRange);
            areaPos = new Vector3(hitPoint.x, 0.01f, hitPoint.z);
            
            // 넓은 범위
            Collider[] detectedObjs = Physics.OverlapSphere(new Vector3(areaPos.x, 0.02f, areaPos.z),15f,_layerMask);
            // 건물이 지어지는 Grid만
            Collider[] constructionObjs = Physics.OverlapBox(new Vector3(areaPos.x, 0.02f, areaPos.z),_range*2,Quaternion.identity,_layerMask);
            UpdateGridMeshToDefault(detectedObjs);
            UpdateOutSelected(constructionObjs);
            // 기존 리스트에서 빠져나갈 요소들의 Mesh값을 바꿔줬으니 새로 받은 Grid 배열을 List로 추가해줌
            detectedGrids = detectedObjs.ToList();
            constructionGrids = constructionObjs.ToList();
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

    private void UpdateOutSelected(Collider[] selectedObjs)
    {
        if(constructionGrids.Count() != 0)
        {
            foreach(Collider obj in constructionGrids)
            {
                if(obj.TryGetComponent(out GridEvent grid))
                {
                    if(!selectedObjs.Contains(obj) && (grid.GetIsBuilted() || grid.GetIsHasObject()))
                    {
                        grid.ChangeMesh();
                    }
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
                if(grid.GetIsBuilted() || grid.GetIsHasObject())
                {
                    grid.ChangeMeshToBlocked();
                } else {
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

    public Vector3 CalculateGridScale(List<Collider> colliders = null)
    {
        List<Collider> checkList = colliders ?? constructionGrids;
        Vector3 center = Vector3.zero;

        foreach(Collider collider in checkList)
        {
            center += collider.gameObject.transform.position;
        }

        return new Vector3(center.x,0,center.z) / checkList.Count();
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

    public void SetAfterDestroy(List<Collider> colliders)
    {
        foreach(Collider obj in colliders)
        {
            if(obj.TryGetComponent(out GridEvent grid))
            {
                if(grid.GetIsBuilted())
                {
                    grid.SetDefault();
                    grid.ChangeMesh();
                }
            }
        }
    }

    public List<Collider> SetStartingPoint(int index)
    {
        List<Collider> colliders = new List<Collider>();
        for(int i=0; i<4; i++)
        {
            Collider startPoint = startingPoints[index].gridList[i];
            if(startPoint.TryGetComponent(out GridEvent grid))
            {
                colliders.Add(startPoint);
                grid.SetBuilted();
                grid.ChangeMesh();
            }
        }
        return colliders;
    }

    public void ActiveFalse()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                startingPoints[i].gridList[i].gameObject.SetActive(false);
            }
        }
    }
}
