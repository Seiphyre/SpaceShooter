using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Asteroid : AEnemy
{
    // ----- [ Attributes ] -----------------------------------------------

    [Header("Asteroid/Movement")]

    [SerializeField]
    protected float _speed = 8;



    // ----- [ Functions ] ------------------------------------------------

    // --v-- Unity Messages --v--

    protected override void Update()
    {
        CalcMovement();

        base.Update();
    }


    // --v-- Movement --v--

    private void CalcMovement()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    }

}
