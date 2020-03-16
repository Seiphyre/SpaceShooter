using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : AEnemy
{
    // ----- [ Attributes ] -------------------------------------------

    //--v-- Movement --v--

    [Header("Turret/Movement")]

    [SerializeField]
    protected float _speed = 8;

    [SerializeField]
    [Range(0f, 100f)]
    protected float _destinationDist = 5;

    private Vector3 _destination;

    private bool _canMove = true;

    // --v-- Weapon --v--

    [Header("Turret/Weapon")]

    [SerializeField]
    private Transform _bulletSpawn = null;

    [SerializeField]
    private Transform _bulletPrefab = null;

    [SerializeField]
    private float _fireRate = 0.5f;

    private float _fireRateCountdown = 0;



    // ----- [ Functions ] -------------------------------------------

    // --v-- Unity Messages --v--

    protected override void Start()
    {
        base.Start();

        _destination = CalcStopDestination();
    }

    protected override void Update()
    {
        UpdateFireCooldown();

        if (_canMove)
            CalcMovement();

        base.Update();

        if (!_canMove && _fireRateCountdown <= 0)
        {
            FireBullet();
        }
    }

    // --v-- Movement --v--

    private void CalcMovement()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * _speed);

        if (transform.position.x < _destination.x)
        {
            transform.position = _destination;
            _canMove = false;
        }
    }

    private Vector3 CalcStopDestination()
    {
        MinMaxBounds mapBoundary = GameManager.Instance.GetMapInfo().Boundary;

        float maxDistance = mapBoundary.Max.x - mapBoundary.Min.x;
        float distance = (maxDistance / 100f) * _destinationDist;

        Vector3 destination = new Vector3(transform.position.x - distance, transform.position.y, transform.position.z);

        return destination;
    }

    // --v-- Weapon --v--

    private void UpdateFireCooldown()
    {
        if (_fireRateCountdown > 0f)
        {
            _fireRateCountdown -= Time.deltaTime;
            _fireRateCountdown = Mathf.Max(0f, _fireRateCountdown);
        }
    }

    private void FireBullet()
    {
        Instantiate(_bulletPrefab, _bulletSpawn.transform.position, _bulletSpawn.transform.rotation);
        _fireRateCountdown = _fireRate;
    }
}
