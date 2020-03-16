using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }


    public void Display()
    {
        gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
}
