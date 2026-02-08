using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuButton : ASimpleButton
{

    [SerializeField] GameObject leaderboardCanvas;
    [SerializeField] GameObject mainMenuCanvas;
    protected override void OnClickCallback()
    {
        OpenMainMenu();
    }
    void Update()
    {

    }

    void OpenMainMenu()
    {
        // Display main menu
        mainMenuCanvas.SetActive(true);
        leaderboardCanvas.SetActive(false);
    }
}
