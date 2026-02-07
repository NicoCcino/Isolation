using UnityEngine;
using UnityEditor;
using WiDiD.Dialogue;

namespace WiDiD.Dialogue.Editor
{
    [CustomEditor(typeof(DialogueScenarioPlayer))]
    public class DialogueScenarioPlayerEditor : UnityEditor.Editor
    {
        // Variable to hold the scenario SO we want to test
        private DialogueScenarioSO _debugScenarioSO;
        private string _jumpToId = "";

        public override void OnInspectorGUI()
        {
            // Draw the standard inspector elements (Script field, etc.)
            DrawDefaultInspector();

            DialogueScenarioPlayer player = (DialogueScenarioPlayer)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Controls (Play Mode Only)", EditorStyles.boldLabel);

            // Guard clause: Most of logic relies on Awake(), so we block it in Edit Mode
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Please enter Play Mode to use the Scenario Player controls.", MessageType.Info);
                return;
            }

            // Check if controller is initialized
            if (player.DialogueScenarioController == null)
            {
                EditorGUILayout.HelpBox("Controller not initialized.", MessageType.Warning);
                return;
            }

            DrawScenarioLoader(player);

            EditorGUILayout.Space(5);
            DrawStatus(player);

            EditorGUILayout.Space(5);
            DrawNavigationControls(player);

            EditorGUILayout.Space(5);
            DrawJumpControls(player);
        }

        private void DrawScenarioLoader(DialogueScenarioPlayer player)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            _debugScenarioSO = (DialogueScenarioSO)EditorGUILayout.ObjectField(
                "Scenario to Load",
                _debugScenarioSO,
                typeof(DialogueScenarioSO),
                false
            );

            if (GUILayout.Button("Set/Reset Scenario"))
            {
                if (_debugScenarioSO != null)
                {
                    player.SetDialogueScenario(_debugScenarioSO.DialogueScenario, 0, true);
                    Debug.Log($"Loaded Scenario: {_debugScenarioSO.name}");
                }
                else
                {
                    Debug.LogWarning("No DialogueScenarioSO assigned in the inspector.");
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawNavigationControls(DialogueScenarioPlayer player)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous Line", GUILayout.Height(30)))
            {
                player.PlayPreviousLine();
            }

            if (GUILayout.Button("Next Line", GUILayout.Height(30)))
            {
                player.PlayNextLine();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawJumpControls(DialogueScenarioPlayer player)
        {
            GUILayout.BeginHorizontal();

            _jumpToId = EditorGUILayout.TextField("Jump to ID:", _jumpToId);

            if (GUILayout.Button("Play", GUILayout.Width(60)))
            {
                if (!string.IsNullOrEmpty(_jumpToId))
                {
                    player.PlayLineWithId(_jumpToId);
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawStatus(DialogueScenarioPlayer player)
        {
            var controller = player.DialogueScenarioController;

            if (controller.CurrentDialogueScenario == null)
            {
                EditorGUILayout.LabelField("Status:", "No Scenario Loaded");
                return;
            }

            int index = controller.CurrentIndex;
            int total = controller.CurrentDialogueScenario.DialogueLines.Length;

            // Prevent out of bounds display logic
            string displayIndex = (index >= 0 && index < total) ? index.ToString() : "Start/End";

            EditorGUILayout.LabelField("Status:", $"Line {displayIndex} / {total - 1}");

            // Show current Line ID if valid
            if (index >= 0 && index < total)
            {
                var currentLine = controller.CurrentDialogueScenario.DialogueLines[index];
                if (currentLine != null)
                {
                    EditorGUILayout.SelectableLabel($"Current ID: {currentLine.LineId}", EditorStyles.miniLabel, GUILayout.Height(15));
                }
            }
        }
    }
}