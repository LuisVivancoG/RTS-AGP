using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SelectionManager : MonoBehaviour //Helps to keep track of which units the player is manipulating at that time. It communicats to UnitsManager
                                                                  //TODO find somewhere else to implement the update function that does not depend on this script having monobehavior
{
    [SerializeField] private int _faction;
    [SerializeField] private UnitsManager _unitsManager;
    private List<UnitsBase> _unitsSelected = new List<UnitsBase>();

    [SerializeField] private LayerMask _clickableMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Camera _playerCam;
    [SerializeField] private RectTransform _areaSelectGuide;

    private Rect _selectionBox;
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;

    private void Start()
    {
        _areaSelectGuide.sizeDelta = Vector2.zero;
        //TODO assign player cam to respective camera
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startPos = Input.mousePosition;
            _selectionBox = new Rect();

            RaycastHit HitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Cast a ray from camera that updates with mouse position

            if (Physics.Raycast(ray, out HitInfo, 1000, _clickableMask))
            {
                var unitClicked = HitInfo.transform.GetComponent<UnitsBase>();
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if(unitClicked.CellUnit.Faction == _faction)
                    {
                        ShiftSelect(unitClicked);
                    }
                }
                else
                {
                    if (unitClicked.CellUnit.Faction == _faction)
                    {
                        ClickSelect(unitClicked);
                    }
                }
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    DeSelectAll();
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if(math.distance(_startPos, Input.mousePosition) > 25) //TODO Finish implementing pyramid collider made by 4 raycast drawn through the camera
                                                                   //then checks and passes all cellUnits with same faction to SelectUnits()
            {
                _endPos = Input.mousePosition;
                DrawGuide();
                DrawSelection();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
            _startPos = Vector2.zero;
            _endPos = Vector2.zero;
            DrawGuide();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit HitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Cast a ray from camera that updates with mouse position

            if (Physics.Raycast(ray, out HitInfo, 1000, _groundMask))
            {
                if (_unitsSelected.Count > 0)
                {
                    var pos = _unitsManager._gameManager.GameGrid.GetCellWorldCenter(HitInfo.point);
                    foreach (var unit in _unitsSelected)
                    {
                        //unit.CellUnit.MoveToTarget(pos);
                        _unitsManager.LocTarget(unit.CellUnit, pos);
                        //_unitsManager.CheckSurrounding(unit);
                    }
                }
                return;
            }
        }
    }

    public void ClickSelect(UnitsBase unitToAdd)
    {
        DeSelectAll();
        _unitsSelected.Add(unitToAdd);
        unitToAdd.OnSelected();
    }
    public void ShiftSelect(UnitsBase unitToAdd)
    {
        if (!_unitsSelected.Contains(unitToAdd))
        {
            _unitsSelected.Add(unitToAdd);
            unitToAdd.OnSelected();
        }
        else
        {
            _unitsSelected.Remove(unitToAdd);
            unitToAdd.OnDeselect();
        }
    }
    public void DragSelect(UnitsBase unitToAdd)
    {

    }
    public void DeSelectAll()
    {
        foreach (var unit in _unitsSelected)
        {
            unit.OnDeselect();
        }

        _unitsSelected.Clear();
    }
    public void Deselect(UnitsBase unitToDeselect)
    {
        unitToDeselect.OnDeselect();
        _unitsSelected.Remove(unitToDeselect);
    }

    void DrawGuide()
    {
        Vector2 boxStart = _startPos;
        Vector2 boxEnd = _endPos;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        _areaSelectGuide.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        _areaSelectGuide.sizeDelta = boxSize;
    }

    void DrawSelection()
    {
        if (Input.mousePosition.x < _startPos.x) //If mouse position is minor than X start pos then is moving left
        {
            _selectionBox.xMin = Input.mousePosition.x;
            _selectionBox.xMax = _startPos.x;
        }
        else //mouse is moving right
        {
            _selectionBox.xMin = _startPos.x;
            _selectionBox.xMax = Input.mousePosition.x;
        }
        if (Input.mousePosition.y < _startPos.y) //If mouse position is minor than Y start pos then is moving down
        {
            _selectionBox.yMin = Input.mousePosition.y;
            _selectionBox.yMax = _startPos.y;
        }
        else //mouse is moving up
        {
            _selectionBox.yMin = _startPos.y;
            _selectionBox.yMax = Input.mousePosition.y;
        }
    }

    void SelectUnits()
    {
        
    }
}
