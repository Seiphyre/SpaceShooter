using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : AEntity
{
    [SerializeField]
    private float _speed = 2f;

    public static int Value = 10;

    private Vector3 _moveDirection;

    private ParticleSystem _particleSys;

    private bool _isInit = false;


    // ----- [ Getters / Setters ] -----------------------------------------------------




    // ----- [ Functions ] -----------------------------------------------------

    protected override void Awake()
    {
        base.Awake();

        _moveDirection = Vector3.left;
        _particleSys = GetComponent<ParticleSystem>();

        transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.back);
    }

    private void Update()
    {
        if (_isInit)
            CalculateMovement();
    }

    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents;
        int nbrOfCollision = 1;

        // Init ParticleCollisionEvent List
        collisionEvents = new List<ParticleCollisionEvent>();

        // Get nbr of collision 
        nbrOfCollision = _particleSys.GetCollisionEvents(other, collisionEvents);

        if (_particleSys.particleCount - nbrOfCollision <= 0)
            DestroySelf();

    }

    // --v-- Init --v--

    public void Init(int nbrOfCrystals)
    {
        // Set nbr of particle
        ParticleSystem.Burst burst;

        burst = _particleSys.emission.GetBurst(0);
        burst.count = nbrOfCrystals;
        _particleSys.emission.SetBurst(0, burst);

        // Play particle
        _particleSys.Play();

        _isInit = true;
    }


    // --v-- Abstract AEntity --v--

    public override MinMaxBounds GetBoundaryPositionOut()
    {
        MinMaxBounds boundaryEntityOut = new MinMaxBounds();
        MinMaxBounds BoundaryMapOut = GameManager.Instance.GetMapInfo().BoundaryOut;

        float extent = _particleSys.shape.radius;

        boundaryEntityOut.Max = new Vector2(BoundaryMapOut.Max.x + extent, BoundaryMapOut.Max.y + extent);
        boundaryEntityOut.Min = new Vector2(BoundaryMapOut.Min.x - extent, BoundaryMapOut.Min.y - extent);

        return boundaryEntityOut;
    }

    public override void Reset()
    {
        DestroySelf();
    }


    // --v-- Movements --v--

    private void CalculateMovement()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * _speed);

        if (IsOutOfBoundary(BoundaryType.OUT))
        {
            DestroySelf();
        }
    }


    // --v-- Destruction --v--

    public void DestroySelf()
    {
        Destroy(transform.gameObject);
    }

}
