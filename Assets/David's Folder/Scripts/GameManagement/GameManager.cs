using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void Quit()
    {
        Application.Quit();
    }
}
