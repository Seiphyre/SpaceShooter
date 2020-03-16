using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public CurvePath path;
    public float duration = 5;
    public bool play = false;

    public bool loop = true;

    private float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = path.EvaluatePosition(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (play && t <= 1)
        {
            Vector3 prevPos = transform.localPosition;
            transform.position = path.EvaluatePosition(t);
            transform.rotation = path.EvaluateRotation(t);

            Vector3 moveDirection = transform.localPosition - prevPos;
            Vector3 lookDirection = transform.forward;

            if (Vector3.Angle(lookDirection, moveDirection) < 90)
                Debug.Log("Avance");
            else
                Debug.Log("Recule");

            t += Time.deltaTime / duration;
        }

        if (loop && t > 1)
            t = 0;
    }
}
