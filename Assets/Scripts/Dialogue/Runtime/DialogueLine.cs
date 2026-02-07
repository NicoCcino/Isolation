using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using NaughtyAttributes;
namespace WiDiD.Dialogue
{
    [System.Serializable]
    public class DialogueLine
    {
        public static DialogueLine Default => new DialogueLine("null", null);

        public DialogueLine(string lineId, LocalizedString localizedLine)
        {
            LineId = lineId;
            LocalizedLine = localizedLine;
        }

        [field: SerializeField, ReadOnly] public string LineId { get; private set; }
        public string DefaultLine => LineId;
        [field: SerializeField] public LocalizedString LocalizedLine { get; private set; }


        public async UniTask<string> GetLineTask()
        {
            if (!LocalizationSettings.InitializationOperation.IsDone)
            {
                Debug.LogWarning("Unity Localization InitializationOperation is running, waiting for InitializationOperation to end.");
                await LocalizationSettings.InitializationOperation;
                Debug.Log($"Resuming GetLineTask for LineId - {LineId}");
            }
            if (LocalizedLine == null)
            {
                Debug.LogWarning($"LocalizedLine of Line {LineId} is null! Returning default line : {DefaultLine}");
                return DefaultLine;
            }
            if (LocalizedLine.IsEmpty)
            {
                Debug.LogWarning($"LocalizedLine of Line {LineId} is empty for Locale {LocalizationSettings.SelectedLocale}! Ensure table entry is filled. Returning default line : {DefaultLine}.");
                return DefaultLine;
            }

            var tcs = new UniTaskCompletionSource<string>();
            LocalizedLine.GetLocalizedStringAsync().Completed += handle =>
            {
                tcs.TrySetResult(handle.Result);
            };
            return await tcs.Task;
        }

#if UNITY_EDITOR
        public void SetLineId(string id)
        {
            LineId = id;
        }
#endif
    }
}