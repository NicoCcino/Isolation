using UnityEngine;

namespace WiDiD.Dialogue
{
    [System.Serializable]
    public class DialogueScenario
    {
        [field: SerializeField] public DialogueLine[] DialogueLines { get; private set; }

        public DialogueScenario(DialogueLine[] dialogueLines)
        {
            DialogueLines = dialogueLines;
        }
    }
}