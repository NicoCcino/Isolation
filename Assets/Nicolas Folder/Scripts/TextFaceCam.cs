using UnityEngine;

public class TextFaceCam : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main; // récupère la caméra principale
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Faire tourner le texte pour qu'il regarde la caméra
        transform.LookAt(
            transform.position + cam.transform.forward,
            cam.transform.up
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
