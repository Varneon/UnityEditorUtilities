using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Varneon.EditorUtilities.Macros
{
    /// <summary>
    /// Simple cleaner for removing empty scripts from hierarchies
    /// </summary>
    public static class EmptyScriptCleaner
    {
        [MenuItem("Varneon/Macros/Delete Empty Scripts")]
        private static void DeleteEmptyScripts()
        {
            Transform selectedTransform = Selection.activeTransform;

            if(selectedTransform == null) { EditorUtility.DisplayDialog("No active selected GameObject", "Please select the root GameObject for deleting the empty scripts first!", "OK"); return; }

            Component[] components = selectedTransform.GetComponentsInChildren<Component>(true).Where(c => c == null).ToArray();

            if (components.Length == 0) { EditorUtility.DisplayDialog("No empty scripts found", "Couldn't find any empty scripts under the selected GameObject!", "OK"); return; }

            if(EditorUtility.DisplayDialog("Delete empty scripts?", $"Are you sure you want to delete {components.Length} empty scripts?", "Yes", "No"))
            {
                Undo.RegisterCompleteObjectUndo(Selection.activeGameObject, "Delete empty scripts");

                foreach (Transform t in selectedTransform.GetComponentsInChildren<Transform>(true))
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                }
            }
        }
    }
}
