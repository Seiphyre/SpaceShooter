using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AEntity
{
    // ----- [ Attributes ] ---------------------------------

    // --v-- Movement --v--

    [SerializeField]
    private float _speed = 10f;

    // --v-- Damage --v--

    [SerializeField, TagSelector]
    private string[] DamageTagFilter = new string[] { };

    private bool _isColliding = false; // Prevent from multiple collisions

    // ----- [ Functions ] ---------------------------------

    // --v-- Unity Messages --v--

    private void Update()
    {
        CalculateMovement();

        if (IsOutOfBoundary(BoundaryType.OUT))
        {
            Destroy(this.gameObject);
        }

        _isColliding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isColliding) return;
        _isColliding = true;

        for (int i = 0; i < DamageTagFilter.Length; i++)
        {
            if (other.tag == DamageTagFilter[i])
            {
                DamageTarget(other.gameObject);
            }
        }
    }

    // --v-- Abstract AEntity --v--

    public override void Reset()
    {
        SelfDestroy();
    }

    // --v-- Functions : Movement --v--

    private void CalculateMovement()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }


    // --v-- Functions : Damage --v--

    private void DamageTarget(GameObject enemy)
    {
        ADamagableEntity damagableEntity = enemy.GetComponent<ADamagableEntity>();
        if (damagableEntity != null)
            damagableEntity.TakeDamage();

        SelfDestroy();
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
