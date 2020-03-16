using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : APanel
{
    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start --v-- 
    protected override void Start()
    {
        base.Start();

        GameManager.Instance.OnGameOverBegin += Show;
    }

    // --v-- Destroy --v--
    private void OnDestroy()
    {
        // Clear Events
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOverBegin -= Show;
    }
}
