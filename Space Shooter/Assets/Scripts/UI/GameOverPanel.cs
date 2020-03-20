using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : APanel
{
    private bool _shouldShowPanel = false;

    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start --v-- 
    protected override void Start()
    {
        base.Start();

        GameManager.Instance.OnGameStop += HandleOnGameStop;
        GameManager.Instance.OnGameIsOver += HandleOnGameIsOver;
    }

    // --v-- Event Handler --v--

    private void HandleOnGameStop()
    {
        if (_shouldShowPanel)
        {
            Show();

            _shouldShowPanel = false;
        }
    }

    private void HandleOnGameIsOver()
    {
        _shouldShowPanel = true;
    }

    // --v-- Destroy --v--
    private void OnDestroy()
    {
        // Clear Events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStop -= HandleOnGameStop;
            GameManager.Instance.OnGameIsOver -= HandleOnGameIsOver;
        }
    }
}
