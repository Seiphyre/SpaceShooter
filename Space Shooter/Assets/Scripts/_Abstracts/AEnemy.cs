using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AEnemy : ADamagableEntity//AEntity//, IDamagable
{
    // ----- [ Attributes ] -------------------------------------------

    [SerializeField]
    private Crystal _crystalPrefab;

    // --v-- Spawn --v--

    protected Spawn _spawn;

    // --v-- Other --v--

    private bool _isColliding = false; // Prevent from multiple collisions



    // ----- [ Getter / Setters ] -------------------------------------

    public Spawn GetSpawn() { return (_spawn); }
    public void SetSpawn(Spawn spawn) { _spawn = spawn; }



    // ----- [ Functions ] ---------------------------------------------

    // --v-- AEntity Override --v--

    /// <summary>
    /// No Padding for enemies.
    /// </summary>
    public override MinMaxBounds GetBoundaryPositionIn()
    {
        MinMaxBounds boundaryEntityIn = new MinMaxBounds();
        MinMaxBounds BoundaryMap = GameManager.Instance.MapInfo.Boundary;

        boundaryEntityIn.Max = new Vector2(BoundaryMap.Max.x - _boundsRenderer.extents.x, BoundaryMap.Max.y - _boundsRenderer.extents.y);
        boundaryEntityIn.Min = new Vector2(BoundaryMap.Min.x + _boundsRenderer.extents.x, BoundaryMap.Min.y + _boundsRenderer.extents.y);

        return boundaryEntityIn;
    }

    // --v-- ADamagableEntity Override --v--

    protected override void DestructionByDamage()
    {
        if (_crystalPrefab != null)
            InstantiateCrystals();

        base.DestructionByDamage();
    }

    // --v-- Unity Messages --v--

    protected virtual void Update()
    {
        if (IsOutOfBoundary(BoundaryType.OUT))
        {
            SelfDestruction();
        }

        _isColliding = false;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (_isColliding) return;
        _isColliding = true;

        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
                OnColisionWithPlayer(player);
            else
                Debug.LogWarning("[Enemy] null reference. Can't find Player component.");
        }
    }

    protected void OnColisionWithPlayer(Player player)
    {
        player.TakeDamage();
        SelfDestruction();
    }

    // --v-- Crystals --v--

    protected void InstantiateCrystals()
    {
        Crystal instance;
        int randomNbr;

        instance = Instantiate(_crystalPrefab, transform.position, transform.rotation);

        randomNbr = UnityEngine.Random.Range(0, 3);

        instance.Init(randomNbr);
    }
}
