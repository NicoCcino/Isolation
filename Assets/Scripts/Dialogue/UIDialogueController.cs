using System;
using UnityEngine;
using UnityEngine.UI;

namespace WiDiD.Dialogue.UI
{
    public class UIDialogueController : MonoBehaviour
    {
        [SerializeField] private Button buttonNextLine;
        [SerializeField] private Button buttonPreviousLine;

        private void OnEnable()
        {
            buttonNextLine.onClick.AddListener(OnButtonNextClicked);
            buttonNextLine.onClick.AddListener(OnButtonPreviousClicked);
        }
        private void OnDisable()
        {
            buttonNextLine.onClick.RemoveListener(OnButtonNextClicked);
            buttonNextLine.onClick.RemoveListener(OnButtonPreviousClicked);
        }
        private void Update()
        {

        }
        private void OnButtonNextClicked()
        {
            DialogueScenarioPlayer.Instance.PlayNextLine();
        }
        private void OnButtonPreviousClicked()
        {
            DialogueScenarioPlayer.Instance.PlayPreviousLine();
        }
    }
}