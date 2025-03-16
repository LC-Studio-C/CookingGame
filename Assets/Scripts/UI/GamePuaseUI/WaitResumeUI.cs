using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitResumeUI : BaseUI
{
    private void Start()
    {
        CookingGameManager.GetInstance().OnPlayerGameResumed += CookingGameManager_OnPlayerGameResumed;
        Hide();
    }

    private void CookingGameManager_OnPlayerGameResumed(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsLocalPlayerResume() == true)
        {
            Show();
        }
    }

    private void Update()
    {
        if (CookingGameManager.GetInstance().IsGamePuased() == false)
        {
            Hide();
        }
    }
}
