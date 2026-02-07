using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace WiDiD.Dialogue
{
    /// <summary>
    /// Controls progress over <see cref="DialogueScenario"/>. Allows to navigate withing the dialogue while ensuring correct behaviours.
    /// </summary>
    public class DialogueScenarioController
    {
        public DialogueScenario CurrentDialogueScenario { get; private set; } = null;
        public int CurrentIndex { get; private set; }
        public float RelativePositionInScenario => CurrentIndex / ((float)CurrentDialogueScenario.DialogueLines.Length - 1);
        public DialogueScenarioController()
        {
        }
        public DialogueLine CurrentLine()
        {
            if (RelativePositionInScenario > 1 || RelativePositionInScenario < 0)
            {
                Debug.LogError("Current Index in DialogueScenarioController is out of bounds! ");
                return DialogueLine.Default;
            }
            return CurrentDialogueScenario.DialogueLines[CurrentIndex];
        }
        public DialogueLine NextLine()
        {
            if (RelativePositionInScenario >= 1)
            {
                Debug.LogWarning($"You tried to load Next Line of DialogueScenario but you reached the end of the scenario at index {CurrentIndex}, returning Default DialogueLine");
                return DialogueLine.Default;
            }
            CurrentIndex++;
            return CurrentDialogueScenario.DialogueLines[CurrentIndex];
        }
        public DialogueLine PreviousLine()
        {
            if (RelativePositionInScenario <= 0)
            {
                Debug.LogWarning($"You tried to load Previous Line of DialogueScenario but scenario is at index {CurrentIndex}, returning Default DialogueLine");
                return DialogueLine.Default;
            }
            CurrentIndex--;
            return CurrentDialogueScenario.DialogueLines[CurrentIndex];
        }
        public DialogueLine SetCurrentLineWithIndex(int index)
        {
            if (index < 0 || index >= CurrentDialogueScenario.DialogueLines.Length)
            {
                Debug.LogWarning($"You tried to SetCurrentLineWithIndex of DialogueScenario with index {index}, but it is out of bounds. Returning Default DialogueLine");
                return DialogueLine.Default;
            }
            CurrentIndex = index;
            return CurrentDialogueScenario.DialogueLines[CurrentIndex];
        }
        public DialogueLine SetCurrentLine(string lineId)
        {
            DialogueLine dialogueLine = CurrentDialogueScenario.DialogueLines.Where(line => line.LineId == lineId).FirstOrDefault();
            if (dialogueLine == null)
            {
                Debug.LogError($"Couldnt find line with Id : {lineId} in DialogueScenario");
                return DialogueLine.Default;
            }
            CurrentIndex = CurrentDialogueScenario.DialogueLines.ToList().IndexOf(dialogueLine);
            return dialogueLine;
        }
        public void SetCurrentDialogueScenario(DialogueScenario dialogueScenario, int startIndex = 0)
        {
            CurrentDialogueScenario = dialogueScenario;
            CurrentIndex = startIndex;
            SetCurrentLineWithIndex(CurrentIndex);
        }
    }
}
