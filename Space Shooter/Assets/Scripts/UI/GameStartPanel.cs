using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStartPanel : APanel
{
    // ----- [ Attributes ] -----------------------------------------------------

    [SerializeField]
    private TextMeshProUGUI text;



    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start --v-- 

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.OnGameStartBegin += Show;
        GameManager.Instance.OnGameStartBegin += DisplayFirstText;
        GameManager.Instance.OnGameStartEnd += DisplaySecondText;
    }

    // --v-- Text Management --v-- 

    private void DisplaySecondText()
    {
        text.text = "GO !!";

        Invoke("Hide", 1);
    }

    private void DisplayFirstText()
    {
        text.text = "Ready ??";
    }

    // --v-- Destroy --v--

    private void OnDestroy()
    {
        // Clear invokes
        if (IsInvoking("Hide"))
            CancelInvoke("Hide");

        // Clear Events 
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStartBegin -= Show;
            GameManager.Instance.OnGameStartBegin -= DisplayFirstText;
            GameManager.Instance.OnGameStartEnd -= DisplaySecondText;
        }
    }
}
