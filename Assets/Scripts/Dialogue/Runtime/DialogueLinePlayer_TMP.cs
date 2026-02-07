using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
namespace WiDiD.Dialogue
{

    public class DialogueLinePlayer_TMP : MonoBehaviour, IStringLinePlayer
    {
        [field: SerializeField] public TMP_Text TextMesh { get; private set; }

        public void PlayStringLine(string dialogueLine)
        {
            TextMesh.text = dialogueLine;
        }

        public async UniTask PlayStringLineTask(string stringLine)
        {
            TextMesh.text = stringLine;
        }
    }
}