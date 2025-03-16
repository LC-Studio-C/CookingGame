using UnityEngine;
using UnityEngine.UI;

public class BurnWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Image warningImage;

    private float burningProgress = .5f;

    private float warningSoundPlayTimer = 0f;
    private float warningPlayInterval = .3f;

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        if (stoveCounter.GetStoveFryState() == StoveCounter.FryingState.Burning)
        {
            if (stoveCounter.GetBurningTimerProgress() > burningProgress)
            {
                Show();
                warningSoundPlayTimer -= Time.deltaTime;
                if (warningSoundPlayTimer < 0f)
                {
                    warningSoundPlayTimer = warningPlayInterval;
                    SoundManager.GetInstance().PlayBurningWarningSound(transform.position);
                }
            }
            else
            {
                Hide();
            }
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        warningImage.gameObject.SetActive(true);
    }

    private void Hide()
    {
        warningImage.gameObject.SetActive(false);
    }
}
