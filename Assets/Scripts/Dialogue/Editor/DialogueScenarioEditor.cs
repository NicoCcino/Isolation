using UnityEngine;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using WiDiD.Dialogue;

namespace WiDiD.Dialogue.Editor
{
    [CustomEditor(typeof(DialogueScenarioSO))]
    public class DialogueScenarioEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Draw the normal Inspector
            base.OnInspectorGUI();

            // Only run logic if user changed something
            if (EditorGUI.EndChangeCheck())
            {
                AutoFillKeys((DialogueScenarioSO)target);
            }
        }

        private void AutoFillKeys(DialogueScenarioSO script)
        {
            if (script.DialogueScenario?.DialogueLines == null) return;

            bool isDirty = false;

            foreach (var dialogueLine in script.DialogueScenario.DialogueLines)
            {
                // Skip if LocalizedLine is missing
                if (dialogueLine?.LocalizedLine == null || dialogueLine.LocalizedLine.IsEmpty)
                    continue;

                string key = null;
                var locLine = dialogueLine.LocalizedLine;
                var tableEntryRef = locLine.TableEntryReference;

                // --- EXTRACT KEY STRING ---
                if (tableEntryRef.ReferenceType == TableEntryReference.Type.Name)
                {
                    key = tableEntryRef.Key;
                }
                else if (tableEntryRef.ReferenceType == TableEntryReference.Type.Id)
                {
                    var collection = LocalizationEditorSettings.GetStringTableCollection(locLine.TableReference);
                    if (collection != null && collection.SharedData != null)
                    {
                        var entry = collection.SharedData.GetEntry(tableEntryRef.KeyId);
                        if (entry != null)
                        {
                            key = entry.Key;
                        }
                    }
                }

                // --- APPLY KEY IF FOUND ---
                if (!string.IsNullOrEmpty(key))
                {
                    // FIX: Check if they match, rather than just if it's empty.
                    // This handles the "List Duplicate" issue because the old ID won't match the new Key.

                    if (dialogueLine.LineId != key)
                    {
                        dialogueLine.SetLineId(key);
                        isDirty = true;
                    }
                }
            }

            // Save changes if we modified anything
            if (isDirty)
            {
                EditorUtility.SetDirty(script);
            }
        }
    }
}