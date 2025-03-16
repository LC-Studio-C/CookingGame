using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject[] childGameObj;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        if (e.fryingStateArgs == StoveCounter.FryingState.Frying || e.fryingStateArgs == StoveCounter.FryingState.Burning)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Hide()
    {
        foreach (var gameObject in childGameObj)
        {
            gameObject.SetActive(false);
        }
    }

    private void Show()
    {
        foreach (var gameObject in childGameObj)
        {
            gameObject.SetActive(true);
        }
    }
}
