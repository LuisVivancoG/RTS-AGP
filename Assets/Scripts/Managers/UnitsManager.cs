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

        bool onGround = Physics.Raycast(ray, out hitInfo, 10000, _groundMask);
        
        if (Input.GetMouseButtonDown(0) && onGround)
        {
            Vector3 positionClicked = hitInfo.point;
            IList<Vector3> path = _gameManager.GameGrid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, _unitsPosition.transform.position, positionClicked);

            //Debug.Log("PathFound.Count:  " + path.Count);

            for (int i = 0; i < path.Count; i++)
            {
                Debug.Log("proceeed to move unit");
                Vector3 targetPos = path[i];
                _unitsPosition.transform.position = Vector3.MoveTowards(_unitsPosition.transform.position, targetPos, _moveSpeed * Time.deltaTime);
            }
        }
        
    }

    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
}
