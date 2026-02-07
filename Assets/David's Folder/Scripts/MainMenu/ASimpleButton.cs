using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public abstract class ASimpleButton : MonoBehaviour
{
    private Button button;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickCallback);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickCallback);
    }
    protected abstract void OnClickCallback();
}
