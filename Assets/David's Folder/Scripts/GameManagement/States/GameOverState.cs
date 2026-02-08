using UnityEngine;
[System.Serializable]
public class GameOverState : AGameState
{
    private float gameOverScreenTime = 10f;
    public AudioClip audioClip;
    public AudioSource source;
    private float timer;
    public override void Enter()
    {
        timer = 0;
        source.PlayOneShot(audioClip);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > gameOverScreenTime)
        {
            GameStateManager.Instance.ChangeState(EGameState.Playing);
        }
    }
}
