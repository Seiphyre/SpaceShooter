using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : APanel
{

    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Show/Hide panel --v--
    public override void Show()
    {
        base.Show();

        GameManager.Instance.PauseGame();
    }

    public override void Hide()
    {
        base.Hide();

        GameManager.Instance.ResumeGame();
    }
}
