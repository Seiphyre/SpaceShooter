using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : APanel
{
    [SerializeField]
    private Button _pauseButton;

    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Show/Hide panel --v--
    public override void Show()
    {
        base.Show();

        GameManager.Instance.PauseGame();

        if (_pauseButton != null)
            _pauseButton.interactable = false;
    }

    public override void Hide()
    {
        base.Hide();

        GameManager.Instance.ResumeGame();

        if (_pauseButton != null)
            _pauseButton.interactable = true;
    }
}
