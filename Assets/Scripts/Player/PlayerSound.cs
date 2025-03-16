using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private PlayerControl player;

    private float playSoundTimer;
    private float playSoundTimeMax = .1f;

    private void Awake()
    {
        player = GetComponent<PlayerControl>();
    }

    private void Update()
    {
        playSoundTimer -= Time.deltaTime;
        if (playSoundTimer < 0)
        {
            playSoundTimer = playSoundTimeMax;
            if (player.IsWalking() == true)
            {
                PlayMoveSound();
            }
        }
    }

    private void PlayMoveSound()
    {
        float volume = 1f;
        SoundManager.GetInstance().PlayFootStep(player.transform.position, volume);
    }
}
