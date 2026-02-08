using UnityEngine;

public class TriggerGameOutcome : MonoBehaviour
{
    public enum E_DefinitiveOutcome
    {
        Success,
        Defeat
    }

    public E_DefinitiveOutcome OutcomeToSet = E_DefinitiveOutcome.Defeat;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider " + other);


        if (OutcomeToSet == E_DefinitiveOutcome.Success)
        {
            if (other.GetComponentInParent<Player>() != null)
            {
                GameOutcomeManager.Instance.Victory("Player Collided with test success box");

            }

        }
    }
}
