using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
    public static PlayerStartPosition Instance { get; private set; }

    public Vector3 Position => transform.position;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}