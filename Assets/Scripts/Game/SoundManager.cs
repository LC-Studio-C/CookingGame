using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private AudioClipSO audioClipSO;

    private static SoundManager instance;

    private float volume = .3f;

    private const string PRE_SOUND_VOLUME = "SoundEffectVolume";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        volume = PlayerPrefs.GetFloat(PRE_SOUND_VOLUME);
    }

    private void Start()
    {
        PlayerControl.OnAnyPickedSomething += PlayerControl_OnAnyPickedSomething;
        BaseCounter.OnGetedSomething += BaseCounter_OnGetedSomething;
        TrashCounter.OnGetedSomething += TrashCounter_OnGetedSomething;
        CuttingCounter.OnCutSomething += CuttingCounter_OnCutSomething;
    }

    private void CuttingCounter_OnCutSomething(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySoundClip(audioClipSO.chop, cuttingCounter.transform.position);
    }

    private void TrashCounter_OnGetedSomething(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySoundClip(audioClipSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnGetedSomething(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySoundClip(audioClipSO.objectDrop, baseCounter.transform.position);
    }

    private void PlayerControl_OnAnyPickedSomething(object sender, System.EventArgs e)
    {
        PlayerControl player = sender as PlayerControl;
        PlaySoundClip(audioClipSO.objectPickUp, player.transform.position);
    }

    private void PlaySoundClip(AudioClip[] audioClips, Vector3 position, float referenceVolume = 1f)
    {
        AudioClip audioClip = audioClips[Random.Range(0, audioClips.Length)];
        AudioSource.PlayClipAtPoint(audioClip, position, referenceVolume * volume);
    }

    private void PlaySoundClip(AudioClip audioClip, Vector3 position, float referenceVolume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, referenceVolume * volume);
    }

    public void PlayFootStep(Vector3 position, float referenceVolume)
    {
        PlaySoundClip(audioClipSO.footStep, position, referenceVolume * volume);
    }

    public void PlayCountdownSound(Vector3 position)
    {
        PlaySoundClip(audioClipSO.warning, position);
    }

    public void PlayBurningWarningSound(Vector3 position)
    {
        PlaySoundClip(audioClipSO.warning, position);
    }

    public void PlayDeliverySuccessSound(Vector3 position)
    {
        PlaySoundClip(audioClipSO.deliverySuccess, position);
    }

    public void PlayDeliveryFailSound(Vector3 position)
    {
        PlaySoundClip(audioClipSO.deliveryFail, position);
    }

    public void SoundVolumeChange()
    {
        volume += .1f;
        if (volume > 1.1f)
        {
            volume = 0f;
        }
        PlayerPrefs.SetFloat(PRE_SOUND_VOLUME, volume);
    }

    public float GetVolume()
    {
        return volume;
    }

    public static SoundManager GetInstance()
    {
        return instance;
    }
}
