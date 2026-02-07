using UnityEngine;

namespace WiDiD.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/DialogueScenarioSO", fileName = "DialogueScenarioSO")]
    public class DialogueScenarioSO : ScriptableObject
    {
        [field: SerializeField] public DialogueScenario DialogueScenario { get; private set; }
    }
}