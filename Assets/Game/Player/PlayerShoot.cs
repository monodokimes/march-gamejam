﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System.Linq;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private GameObject _lineRenderer;
    [SerializeField]
    private float _lineLength;
    [SerializeField]
    private float _lineHeight;
    [SerializeField]
    private GameObject _projectile;
    [SerializeField]
    private Transform _projectileSpawn;
    [SerializeField]
    private int _shotChargeCost;
    [SerializeField]
    private float _totalCharge;
    [SerializeField]
    private float _chargeSpeed;
    [SerializeField]
    private float _shotDelay;
    [SerializeField]
    private GameObject[] _lights;
    [SerializeField]
    private float _maxLightIntensity;
    [SerializeField]
    private float _lightUpSpeed;
    
    public bool IsAttacking { get; private set; }
    public float CooldownPercent { get { return _currentCharge / _totalCharge; } }
    public bool CanAttack { get { return _currentCharge > _shotChargeCost; } }

    private LineRenderer _line;
    private SFXManager _sfx;
    private IControlMode _input;
    private bool _active;
    private bool _onCooldown;
    private float _currentCharge;
    private bool _isFiring;
    private float _initialLightIntensity;
    private CameraController _camera;
    
    void Start()
    {
        _camera = Camera.main.GetComponent<CameraController>();
        _sfx = this.Find<SFXManager>(GameTags.Sound);
        _currentCharge = _totalCharge;
        _input = GetComponentInChildren<IControlMode>();
        _line = _lineRenderer.GetComponent<LineRenderer>();
        if (_line == null || _input == null)
            throw new Exception();

        var lights = _lights.Select(go => go.GetComponent<Light>());
        _initialLightIntensity = lights.First().intensity;

        StartCoroutine(FadeLights());
    }

    void Update()
    {
        if (_input.IsAiming && !_active)
        {
            _active = true;
            _anim.SetBool("kneel", true);

        }
        if (!_input.IsAiming && _active)
        {
            _active = false;
            _anim.SetBool("kneel", false);
        }

        if (_active)
        {
            var start = transform.position + Vector3.up * _lineHeight;
            var end = start + _input.AimDirection * _lineLength;
            _line.SetPositions(new[] { start, end });
        }
        else
        {
            _line.SetPositions(new [] { Vector3.zero, Vector3.zero });
        }
        if (_currentCharge < _totalCharge)
        {
            _currentCharge += WibblyWobbly.deltaTime * _chargeSpeed;
        }
    }

    public void Recharge()
    {
        _currentCharge = _totalCharge;
    }

    private IEnumerator FadeLights()
    {
        var start = Time.time;
        var lights = _lights.Select(go => go.GetComponent<Light>());

        while (true)
        {
            foreach (var light in lights)
            {
                if (_input.IsAiming)
                {
                    light.intensity = Mathf.Lerp(light.intensity, _maxLightIntensity, _lightUpSpeed);
                }
                else
                {
                    light.intensity = Mathf.Lerp(light.intensity, _initialLightIntensity, _lightUpSpeed);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void Fire()
    {
        if (_isFiring)
            return;

        StartCoroutine(ContinuousFire());
    }

    private IEnumerator ContinuousFire()
    {
        _isFiring = true;
        while (_input.IsFiring)
        {
            _camera.ShakeForSeconds(_shotDelay);
            if (_currentCharge < _shotChargeCost)
            {
                _sfx.PlayRandomSound("laser_empty", 5);
                break;
            }

            _anim.SetTrigger("gunFire");
            _sfx.PlayRandomSound("laser", 10);

            Instantiate(_projectile, _projectileSpawn.position, _projectileSpawn.rotation);

            _currentCharge -= _shotChargeCost;
            yield return new WaitForSeconds(_shotDelay);
        }
        _isFiring = false;
    }

    public void Reload()
    {
        _currentCharge = _totalCharge;
    }

    public bool IsCharged ()
    {
        return _currentCharge >= _totalCharge;
    }
}
