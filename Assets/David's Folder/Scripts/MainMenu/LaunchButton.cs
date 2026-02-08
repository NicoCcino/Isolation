using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class StartButton : ASimpleButton
{
    protected override void OnClickCallback()
    {
        StartGame();
    }
    void Update()
    {
        // If player presses E, start game.
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        GameStateManager.Instance.ChangeState(EGameState.Playing);
    }

}
