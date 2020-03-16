using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStartPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    void Start()
    {
        GameManager.Instance.OnGameStartBegin += Display;
        GameManager.Instance.OnGameStartEnd += ShowSecondText;

        gameObject.SetActive(false);
    }


    public void Display()
    {
        gameObject.SetActive(true);
    }

    public void ShowSecondText()
    {
        text.text = "GO !!";

        Invoke("Hide", 1);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
