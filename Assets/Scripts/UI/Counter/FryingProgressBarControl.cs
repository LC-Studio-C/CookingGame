using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FryingProgressBarControl : MonoBehaviour
{
    [SerializeField] private StoveCounter fryingCounter;
    [SerializeField] private Image progressBar;
    private Animator animator;

    private IProgress progress;

    private const string WARNING = "Warning";

    private float burningProgress = .5f;

    private void Start()
    {
        if (fryingCounter != null)
        {
            progress = fryingCounter;
        }
        progress.OnProgressChanged += Progress_OnProgressChanged;
        progressBar.fillAmount = 0;
        animator = GetComponent<Animator>();
        Hide();
    }

    private void Update()
    {
        if (fryingCounter.GetStoveFryState() == StoveCounter.FryingState.Burning)
        {
            if (fryingCounter.GetBurningTimerProgress() > burningProgress)
            {
                animator.SetBool(WARNING, true);
            }
            else
            {
                animator.SetBool(WARNING, false);
            }
        }
        else
        {
            animator.SetBool(WARNING, false);
        }
    }

    private void Progress_OnProgressChanged(object sender, IProgress.OnProgressChangedEventArgs e)
    {
        progressBar.fillAmount = e.progressArgs;
        if (e.progressArgs == 0f || e.progressArgs >= 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    /*private void FryingCounter_OnFryProgressChanged(object sender, StoveCounter.FryProgressChangedEventArgs e)
    {
        *//*switch (e.fryingStateArgs)
        {
            case StoveCounter.FryingState.Frying:
                progressBar.color = new Color(0.9339623f, 0.5697253f, 0.3759344f, 1f);
                break;
            case StoveCounter.FryingState.Burning:
                progressBar.color = Color.red;
                break;
        }*/

        /*progressBar.fillAmount = e.fryProgressArgs;
        if (e.fryProgressArgs == 0f || e.fryProgressArgs >= 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }*//*
    }*/

    /// <summary>
    /// 
    /// </summary>
    private void Hide()
    {
        gameObject.SetActive(false);
    }


    /// <summary>
    /// 
    /// </summary>
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
