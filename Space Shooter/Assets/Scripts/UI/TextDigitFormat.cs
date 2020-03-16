using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDigitFormat : MonoBehaviour
{
    [SerializeField]
    private string _format = "{0:0000}";

    public string Format { get { return _format; } }
}
