using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool isFrying = e.fryingStateArgs == StoveCounter.FryingState.Frying || e.fryingStateArgs == StoveCounter.FryingState.Burning;
        if (isFrying == true)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
