using UnityEngine;
using System.Collections;

public class TagSelectorExample : MonoBehaviour
{
    [TagSelector]
    public string TagFilter = "";

    [TagSelector]
    public string[] TagFilterArray = new string[] { };
}