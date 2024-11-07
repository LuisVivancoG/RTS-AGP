using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _unitsPosition;
    [SerializeField] private Vector3 _objectivePosition;

    [SerializeField] private float _moveSpeed;

    [SerializeField] private LayerMask _groundMask;

    private void Update()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Cast a ray from camera that updates with mouse position

        bool canMove = Physics.Raycast(ray, out hitInfo, 10000, _groundMask);

        if (Input.GetMouseButtonDown(2))
        {
            _gameManager.GameGrid.Pathfinder.StatusCheck();
        }

            if (Input.GetMouseButtonDown(0) && canMove)
        {
            Debug.Log("raycast hit layer");
            Vector3 positionClicked = hitInfo.point;
            _gameManager.GameGrid.Pathfinder.FindShortestPathAStar(_unitsPosition.transform.position, positionClicked, "manhattan");
            _gameManager.GameGrid.Pathfinder._currentPathIndex = 0;

            Debug.Log("CurrentPathIndex:  " + _gameManager.GameGrid.Pathfinder._currentPathIndex + " - " + "PathFound.Count:  " + _gameManager.GameGrid.Pathfinder._currentPathFound.Count);
        }
        if (_gameManager.GameGrid.Pathfinder._currentPathFound != null && _gameManager.GameGrid.Pathfinder._currentPathIndex < _gameManager.GameGrid.Pathfinder._currentPathFound.Count) //if there is a path set and the unit hasn't reach it yet then move
        {
            Debug.Log("proceeed to move unit");
            Vector3 targetPos = _gameManager.GameGrid.Pathfinder._currentPathFound[_gameManager.GameGrid.Pathfinder._currentPathIndex];
            _unitsPosition.transform.position = Vector3.MoveTowards(_unitsPosition.transform.position, targetPos, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(_unitsPosition.transform.position, targetPos) < 0.1f)
            {
                _gameManager.GameGrid.Pathfinder._currentPathIndex++;
            }
        }
    }

    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
}
