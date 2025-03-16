using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToStartUI : BaseUI
{

    private void Start()
    {
        CookingGameManager.GetInstance().OnPlayerReadyChanged += CookingGameManager_OnPlayerReadyChanged;
        CookingGameManager.GetInstance().OnGameStateChanged += CookingGameManager_OnGameStateChanged;
        Hide();
    }

    private void CookingGameManager_OnGameStateChanged(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsWaitingToStart() == false)
        {
            Hide();
        }
    }

    private void CookingGameManager_OnPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsLocalPlayerReady())
        {
            Show();
        }
    }
}
