using System.Linq;
using Cysharp.Threading.Tasks;
namespace WiDiD.Dialogue
{
    public class DialogueScenarioPlayer : Singleton<DialogueScenarioPlayer>
    {
        public DialogueScenarioController DialogueScenarioController { get; private set; }
        public IStringLinePlayer[] StringLinePlayers { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            DialogueScenarioController = new DialogueScenarioController();
            StringLinePlayers = GetComponentsInChildren<IStringLinePlayer>();
        }

        public async void SetDialogueScenario(DialogueScenario dialogueScenario, int startIndex = 0, bool playLine = false)
        {
            DialogueScenarioController.SetCurrentDialogueScenario(dialogueScenario, startIndex);
            if (playLine)
                PlayCurrentLine();
        }
        public async UniTask SetDialogueScenarioTask(DialogueScenario dialogueScenario, int startIndex = 0, bool playLine = false)
        {
            DialogueScenarioController.SetCurrentDialogueScenario(dialogueScenario, startIndex);
            if (playLine)
                await PlayCurrentLineTask();
        }

        public async UniTask PlayNextLineTask()
        {
            DialogueLine dialogueLine = DialogueScenarioController.NextLine();
            await WaitForAllStringLinePlayersTask(await dialogueLine.GetLineTask());
        }
        public async void PlayNextLine()
        {
            DialogueLine dialogueLine = DialogueScenarioController.NextLine();
            PlayLineStringLinePlayers(await dialogueLine.GetLineTask());
        }
        public async UniTask PlayPreviousLineTask()
        {
            DialogueLine dialogueLine = DialogueScenarioController.PreviousLine();
            await WaitForAllStringLinePlayersTask(await dialogueLine.GetLineTask());
        }
        public async void PlayPreviousLine()
        {
            DialogueLine dialogueLine = DialogueScenarioController.PreviousLine();
            PlayLineStringLinePlayers(await dialogueLine.GetLineTask());
        }
        public async UniTask PlayLineWithIdTask(string lineId)
        {
            DialogueLine dialogueLine = DialogueScenarioController.SetCurrentLine(lineId);
            await WaitForAllStringLinePlayersTask(await dialogueLine.GetLineTask());
        }
        public async void PlayLineWithId(string lineId)
        {
            DialogueLine dialogueLine = DialogueScenarioController.SetCurrentLine(lineId);
            PlayLineStringLinePlayers(await dialogueLine.GetLineTask());
        }
        public async UniTask PlayCurrentLineTask()
        {
            DialogueLine dialogueLine = DialogueScenarioController.CurrentLine();
            await WaitForAllStringLinePlayersTask(await dialogueLine.GetLineTask());
        }
        public async void PlayCurrentLine()
        {
            DialogueLine dialogueLine = DialogueScenarioController.CurrentLine();
            PlayLineStringLinePlayers(await dialogueLine.GetLineTask());
        }
        private async UniTask WaitForAllStringLinePlayersTask(string stringLine)
        {
            if (string.IsNullOrEmpty(stringLine) || stringLine == "null") return;

            if (StringLinePlayers == null || StringLinePlayers.Length == 0) return;

            var tasks = StringLinePlayers.Select(p => p.PlayStringLineTask(stringLine)).ToArray();

            await UniTask.WhenAll(tasks);
        }
        private void PlayLineStringLinePlayers(string stringLine)
        {
            if (string.IsNullOrEmpty(stringLine) || stringLine == "null") return;

            foreach (IStringLinePlayer stringLinePlayer in StringLinePlayers)
            {
                stringLinePlayer.PlayStringLine(stringLine);
            }
        }

    }
}