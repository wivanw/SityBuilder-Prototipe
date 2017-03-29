using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _camera;
    private Transform _transform;
    private Quaternion _turnY;
    public float CameraSpeed = 1;
    private Vector3 _camPosCorrection;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _transform = GetComponent<Transform>();
        _turnY = Quaternion.Euler(0, _transform.rotation.eulerAngles.y, 0);
        _camPosCorrection = CulcCamCorrection();

    }

    private void Update()
    {
        var addSize = -Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetMouseButton(0))
        {
            var rotationHorizonyal = -Input.GetAxis("Mouse X") * Time.deltaTime * 50 * CameraSpeed;
            var rotationVertical = -Input.GetAxis("Mouse Y") * Time.deltaTime * 50 * CameraSpeed;
            _transform.position += _turnY * new Vector3(rotationHorizonyal, 0, rotationVertical);
        }
        if (Math.Abs(addSize) > float.Epsilon)
        {
            _camera.orthographicSize = Mathf.Clamp(
                _camera.orthographicSize + addSize * Time.deltaTime * 400, 0.5f, 13.5f);
        }
        if (Input.GetMouseButton(0) || Math.Abs(addSize) > float.Epsilon)
            ClampCamPos();
    }

    private void ClampCamPos()
    {
        var rayCenter = _transform.position + _camPosCorrection;
        var orthogCoef = Constants.WorldSize - _camera.orthographicSize + 1;
        var rayCenterClamp = new Vector3(
            Mathf.Clamp(rayCenter.x, _camera.orthographicSize, orthogCoef),
            rayCenter.y,
            Mathf.Clamp(rayCenter.z, _camera.orthographicSize, orthogCoef));
        _transform.position += rayCenterClamp - rayCenter;
    }


    private Vector3 CulcCamCorrection()
    {
        var screenToWorldPoint = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0.0f));
        return Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width / 2f, Screen.height / 2f,
            screenToWorldPoint.y / Mathf.Cos(_transform.rotation.eulerAngles.x * Mathf.Deg2Rad)))
            - screenToWorldPoint;
    }
}
