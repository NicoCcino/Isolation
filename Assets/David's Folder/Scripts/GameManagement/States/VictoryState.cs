using UnityEngine;
[System.Serializable]
public class VictoryState : AGameState
{
    private float victoryScreenTime = 6.0f;
    public AudioClip victoryClip;
    public AudioSource audioSource;
    private float timer;
    public override void Enter()
    {
        timer = 0;
        audioSource.PlayOneShot(victoryClip);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > victoryScreenTime)
        {
            GameStateManager.Instance.ChangeState(EGameState.MainMenu);
        }
    }
}
