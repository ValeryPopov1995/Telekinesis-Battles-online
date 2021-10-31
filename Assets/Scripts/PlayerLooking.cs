using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLooking : MonoBehaviour
{
    [SerializeField] float _sensitivity = 100;
    [SerializeField] Transform _camera, _body;
    [SerializeField] PhotonView _view;

    void Update()
    {
        if (!_view.IsMine) return;
        Vector2 rotationDirection = Input.GetAxis("Mouse X") * Vector2.right + Input.GetAxis("Mouse Y") * Vector2.up;
        rotationDirection *= _sensitivity * Time.deltaTime;

        _camera.Rotate(_camera.right, -rotationDirection.y, Space.World);
        float xCameraRotation = Mathf.Clamp(_camera.rotation.eulerAngles.x, 0, 360);
        _camera.localRotation = Quaternion.Euler(xCameraRotation, 
            _camera.rotation.y, 
            _camera.rotation.z);

        _body.Rotate(_body.up, rotationDirection.x);
    }
}
