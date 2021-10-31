using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerParameters : MonoBehaviour
{
    [SerializeField] float _maxHealth, _minVelocityDamage, _multiplyDamageByVelocity;
    [SerializeField] Slider _healthBar;
    [SerializeField] Transform _body;
    [SerializeField] PhotonView _view;

    float _health;

    private void Awake()
    {
        _health = _maxHealth;

        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _health;

        if (!_view.IsMine)
        {
            _healthBar.gameObject.SetActive(false);
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_view.IsMine) return;
        if (collision.relativeVelocity.magnitude < _minVelocityDamage) return;

        float damage = collision.relativeVelocity.magnitude * _multiplyDamageByVelocity;
        _health -= damage;
        if (_health >= 0) _healthBar.value = _health;
        if (_health <= 0) die();
    }

    void die()
    {
        _health = _maxHealth;
        _healthBar.value = _health;
        _body.position = GameManager.SetPlayerPos();
    }
}
