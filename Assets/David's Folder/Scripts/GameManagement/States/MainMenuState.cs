using UnityEngine;
[System.Serializable]
public class MainMenuState : AGameState
{
    public override void Enter()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }
}
