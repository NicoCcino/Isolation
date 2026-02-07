using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    private float speed;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = Player.Instance.KinematicCarController.Motor.BaseVelocity.magnitude;

    }

    // Update is called once per frame
    void Update()
    {
        speed = Player.Instance.KinematicCarController.Motor.BaseVelocity.magnitude;

        // Paramètres Animator
        animator.SetFloat("Speed", speed);
        //animator.SetBool("IsMoving", speed > 0.1f);
    }
}
