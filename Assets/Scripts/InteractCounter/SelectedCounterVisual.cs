using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    public BaseCounter baseCounter;
    public List<GameObject> childGameObjectList = new List<GameObject>();

    private void Start()
    {
        if (PlayerControl.GetLocalInstance() != null)
        {
            PlayerControl.GetLocalInstance().OnSelectedCounterChanged += PlayerControl_OnSelectedCounterChanged;
        }
        else
        {
            PlayerControl.OnAnyPlayerSpawned += PlayerControl_OnAnyPlayerSpawned;
        }
    }

    private void PlayerControl_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (PlayerControl.GetLocalInstance() != null)
        {
            PlayerControl.GetLocalInstance().OnSelectedCounterChanged -= PlayerControl_OnSelectedCounterChanged;
            PlayerControl.GetLocalInstance().OnSelectedCounterChanged += PlayerControl_OnSelectedCounterChanged;
        }
    }

    private void PlayerControl_OnSelectedCounterChanged(object sender, PlayerControl.OnSelectedCounterChangedEventArgs e)
    {
        if (baseCounter == e.selectedCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (var childGameObject in childGameObjectList)
        {
            childGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (var childGameObject in childGameObjectList)
        {
            childGameObject.SetActive(false);
        }
    }
}
