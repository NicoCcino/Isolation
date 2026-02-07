using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnnemyAudio : MonoBehaviour
{
    private AudioSource audioSource = null;
    [SerializeField] private AudioClip[] footSteps;
    [SerializeField] private float footStepVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayOneShotFootStep()
    {
        AudioClip randomClip = footSteps[Random.Range(0, footSteps.Length)];
        audioSource.PlayOneShot(randomClip, footStepVolume);
    }
}
