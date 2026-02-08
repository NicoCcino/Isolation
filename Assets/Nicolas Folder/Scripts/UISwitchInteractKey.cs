using UnityEngine;

public class UISwitchInteractKey : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] public GameObject lightSwitchGO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnValidate()
    {
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateUISwitchInteractKeyPosition()
    {
        if (lightSwitchGO == null || cam == null)
            return;

        // Position monde écran
        Vector3 screenPos = cam.WorldToScreenPoint(lightSwitchGO.transform.position);

        // Si l'objet est derrière la caméra, on cache la UI
        if (screenPos.z < 0)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);
        this.transform.position = screenPos;
    }
}
