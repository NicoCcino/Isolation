using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    private AudioSource audioSource = null;
    [SerializeField] private AudioClip[] footSteps;
    [SerializeField] private float footStepVolume;
    [SerializeField] private AudioClip[] warnedReactions;
    [SerializeField] private float warnedReactionVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayOneShotFootStep()
    {
        AudioClip randomClip = footSteps[Random.Range(0, footSteps.Length)];
        audioSource.PlayOneShot(randomClip, footStepVolume);
    }
    public void PlayWarnedSound()
    {
        AudioClip randomClip = warnedReactions[Random.Range(0, warnedReactions.Length)];
        audioSource.PlayOneShot(randomClip, warnedReactionVolume);
    }
}
