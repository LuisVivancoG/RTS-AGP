using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float _zoomSpeed = 100f;
    [SerializeField] private float _zoomTime = 0.1f;

    [SerializeField] private float _maxHeight = 100f;
    [SerializeField] private float _minHeight = 20f;

    [SerializeField] private float _focusHeight = 10f;
    [SerializeField] private float _focusDistance = 20f;

    [SerializeField] private int _panBorder = 25;
    [SerializeField] private float _dragPanSpeed = 25f;
    [SerializeField] private float _edgePanSpeed = 25f;
    [SerializeField] private float _keyPanSpeed = 25f;

    [SerializeField] private float _updateAngleHeight = 200;
    [SerializeField] private float _minRotation = 25;
    [SerializeField] private float _maxRotation = 90;

    private float _zoomVelocity = 0f;
    private float _targetHeight;

    private void Start()
    {
        // Start zoomed out
        _targetHeight = _maxHeight;
    }

    private void Update()
    {
        var newPosition = transform.position;

        // First, calculate the height we want the camera to be at
        _targetHeight += Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed * -1f;
        _targetHeight = Mathf.Clamp(_targetHeight, _minHeight, _maxHeight);

        // Then, interpolate smoothly towards that height
        newPosition.y = Mathf.SmoothDamp(transform.position.y, _targetHeight, ref _zoomVelocity, _zoomTime);

        // Always pan the camera using the keys
        var pan = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * _keyPanSpeed * Time.deltaTime;

        // Optionally pan the camera by either dragging with middle mouse or when the cursor touches the screen border
        if (Input.GetMouseButton(2))
        {
            pan -= new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * _dragPanSpeed * Time.deltaTime;
        }
        else
        {
            var border = Vector2.zero;
            if (Input.mousePosition.x < _panBorder) border.x -= 1f;
            if (Input.mousePosition.x >= Screen.width - _panBorder) border.x += 1f;
            if (Input.mousePosition.y < _panBorder) border.y -= 1f;
            if (Input.mousePosition.y > Screen.height - _panBorder) border.y += 1f;
            pan += border * _edgePanSpeed * Time.deltaTime;
        }

        newPosition.x += pan.x;
        newPosition.z += pan.y;

        var focusPosition = new Vector3(newPosition.x, _focusHeight, newPosition.z + _focusDistance);

        transform.position = newPosition;

        var targetRotation = transform.rotation;

        // Rotate the camera based on the focusPosition.z value
        if (transform.position.y <= _updateAngleHeight)
        {
            // Rotate to 25 degrees on the X-axis
            targetRotation = Quaternion.Euler(_minRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        else
        {
            // Default rotation towards the focusPosition
            targetRotation = Quaternion.Euler(_maxRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
    }
}