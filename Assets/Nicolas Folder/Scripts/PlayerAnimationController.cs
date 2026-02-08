using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    private float speed;
    private bool goingForward;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = Player.Instance.KinematicCarController.Motor.BaseVelocity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGoingForward()){
        speed = Player.Instance.KinematicCarController.Motor.BaseVelocity.magnitude;
        }
        else{
        speed = - Player.Instance.KinematicCarController.Motor.BaseVelocity.magnitude;
        }


        // Paramètres Animator
        animator.SetFloat("Speed", speed);
        //animator.SetBool("IsMoving", speed > 0.1f);
    }

    private bool isGoingForward()
    {
        float prod = Vector3.Dot(Player.Instance.KinematicCarController.Motor.BaseVelocity,-Player.Instance.transform.forward);
        if (prod <= 0)
        {
            return false;
        }
            return true;
    }
}
