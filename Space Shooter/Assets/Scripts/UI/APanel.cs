using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APanel : MonoBehaviour
{
    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start --v-- 

    protected virtual void Start()
    {
        gameObject.SetActive(false);
    }

    // --v-- Show/Hide Panel --v-- 

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
