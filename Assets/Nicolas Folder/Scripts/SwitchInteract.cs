using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchInteract : MonoBehaviour
{

    public bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleLight();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInRange = true;
            Debug.Log("Player dans la zone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInRange = false;
            Debug.Log("Player sorti de la zone");
        }
    }

    void ToggleLight()
    {
        Debug.Log ("Light switch toggled by player in range");
    }

}
