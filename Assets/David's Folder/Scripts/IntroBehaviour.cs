using UnityEngine;
using WiDiD.UI;

public class IntroBehaviour : MonoBehaviour
{
    [SerializeField] private CanvasGroupCustom canvasGroupCustom;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip introClip;
    public static bool hasPlayedIntro = false;
    public float duration = 19;
    private void OnEnable()
    {
        if (hasPlayedIntro)
        {

            Invoke("DisableTuto", 10);
            return;
        }

        canvasGroupCustom.Fade(true);
        audioSource.PlayOneShot(introClip, 0.6f);

        Invoke("DisableTuto", duration);
    }
    private void DisableTuto()
    {
        canvasGroupCustom.Fade(false);
        hasPlayedIntro = true;
    }
}
