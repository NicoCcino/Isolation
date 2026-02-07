using UnityEngine;

public class QuitButton : ASimpleButton
{
    protected override void OnClickCallback()
    {
        GameManager.Instance.Quit();
    }
}
