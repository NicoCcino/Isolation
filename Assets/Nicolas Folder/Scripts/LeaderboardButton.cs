using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeaderboardButton : ASimpleButton
{

    [SerializeField] GameObject leaderboardCanvas;
    [SerializeField] GameObject mainMenuCanvas;
    protected override void OnClickCallback()
    {
        OpenLeaderboard();
    }
    void Update()
    {
        // If player presses L, go to leaderboard.
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenLeaderboard();
        }
    }

    void OpenLeaderboard()
    {
        // Display leaderboard
        leaderboardCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }
}
