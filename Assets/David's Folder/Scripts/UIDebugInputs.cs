using UnityEngine.UI;
using UnityEngine;

public class UIDebugInputs : MonoBehaviour
{
    [System.Serializable]
    private struct InputKey
    {
        public Vector2 direction;
        public Image image;
    }

    [SerializeField] private InputKey[] moveInputKeys;

    public void UpdateWithMoveInput(Vector2 moveInput)
    {
        foreach (InputKey inputKey in moveInputKeys)
        {
            if (inputKey.image == null)
                continue;


            float dot = Vector2.Dot(moveInput, inputKey.direction);

            // Check if the input matches this key's direction
            bool isPressed = dot > 0;

            inputKey.image.color = isPressed ? Color.white : Color.white * 0.5f;
        }
    }
}
