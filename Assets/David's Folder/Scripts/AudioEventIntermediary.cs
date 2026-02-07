using UnityEngine;

public class AudioEventIntermediary : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 5.0f;
    [SerializeField] private AudioLoopBlender audioLoopBlender;
    [SerializeField] private AudioClip[] hitAudioClips;
    [SerializeField] private float volume = 1.0f;
    private float cooldownTimer;
    public void PlayAudioEvent()
    {
        if (cooldownTimer > 0) return;

        AudioClip randomClip = hitAudioClips[Random.Range(0, hitAudioClips.Length)];
        audioLoopBlender.PlayOverrideEvent(randomClip, volume);
        cooldownTimer = cooldownTime;
    }
    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }
}
