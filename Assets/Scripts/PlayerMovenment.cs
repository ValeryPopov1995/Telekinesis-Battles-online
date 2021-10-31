using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovenment : MonoBehaviour
{
    [SerializeField] float _speed = 600, _jump = 100;
    [SerializeField] Rigidbody _body;
    [SerializeField] PhotonView _view;
    [SerializeField] LayerMask _notBodylayers;

    Transform _transformBody;

    void Awake()
    {
        _transformBody = _body.transform;
        _body.freezeRotation = true;
        if (!_view.IsMine) enabled = false;
    }

    void Update()
    {
        //if (!_view.IsMine) return;
        const float checkSphere = .5f;

        if (!Input.GetButtonDown("Jump")) return;
        if (Physics.CheckSphere(_body.position + Vector3.down, checkSphere, _notBodylayers)) _body.AddForce(Vector3.up * _jump);
    }

    void FixedUpdate()
    {
        if (!_view.IsMine) return;
        Vector3 horizontalVelocity = 
            (Input.GetAxis("Vertical") + Input.GetAxis("Run")) * _transformBody.forward + 
            Input.GetAxis("Horizontal") * _transformBody.right;
        horizontalVelocity *= _speed * Time.fixedDeltaTime;
        _body.velocity = horizontalVelocity + _transformBody.up * _body.velocity.y;
    }
}
