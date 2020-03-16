using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn
{
    // ----- [ Attributes ] -------------------------------------------

    // --v-- Public Attributes --v--

    public int Id { get; private set; }

    public Vector3 Position { get; private set; }

    public Vector2 Size { get; private set; }

    // --v-- Private Attributes --v--

    private AEnemy _enemy;



    // ----- [ Getters / Setters ] --------------------------------------



    public void SetEnemy(AEnemy enemy)
    {
        _enemy = enemy;
        _enemy.OnSelfDestroy += RemoveEnemy;
    }



    // ----- [ Constructors ] -------------------------------------------



    public Spawn(Vector2 pos, Vector2 size, int id)
    {
        Position = new Vector3(pos.x, pos.y, 0f);
        Size = size;

        Id = id;

        _enemy = null;
    }



    // ----- [ Functions] -----------------------------------------------



    // --v-- Public Functions --v--

    public bool IsAvailable()
    {
        return (_enemy == null ? true : false);
    }


    public AEnemy Instanciate(AEnemy prefabEnemy)
    {
        Transform instance;
        AEnemy instanciatedEnemy;

        instance = Object.Instantiate(prefabEnemy.transform, Position, Quaternion.LookRotation(Vector3.left, Vector3.up));
        instanciatedEnemy = instance.GetComponent<AEnemy>();

        if (instanciatedEnemy != null)
        {
            instanciatedEnemy.transform.Translate(-instanciatedEnemy.GetExtents().Min.x, 0, 0f, Space.World);

            SetEnemy(instanciatedEnemy);
            instanciatedEnemy.SetSpawn(this);
        }
        else
        {
            Object.Destroy(instance);
            Debug.LogWarning("[Spawn] [Instanciate] Null reference. Cannot assign new enemy to spawn.");

            return null;
        }

        return instanciatedEnemy;
    }

    // --v-- Private Functions --v--

    private void RemoveEnemy()
    {
        _enemy = null;
    }
}
