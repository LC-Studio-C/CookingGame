using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuttingProgressBarControl : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private Image progressBar;
    private IProgress progress;

    private void Start()
    {
        if (cuttingCounter != null)
        {
            progress = cuttingCounter;
        }
        progress.OnProgressChanged += Progress_OnProgressChanged;
        progressBar.fillAmount = 0;

        Hide();
    }

    private void Progress_OnProgressChanged(object sender, IProgress.OnProgressChangedEventArgs e)
    {
        progressBar.fillAmount = e.progressArgs;
        if (e.progressArgs == 0f || e.progressArgs == 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

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




    /*private void CuttingCounter_OnCuttingProgressChanged(object sender, CuttingCounter.CuttingProgressEventArgs e)
    {
        progressBar.fillAmount = e.cuttingProgressArgs;
        if (e.cuttingProgressArgs == 0f || e.cuttingProgressArgs == 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }*/
}
