using UnityEditor;
using UnityEngine;

namespace Varneon.EditorUtilities.HierarchyActions
{
    /// <summary>
    /// Collection of hierarchy context menu actions for creating proxy objects
    /// </summary>
    public static class HierarchyContextMenuActions
    {
        /// <summary>
        /// Select dependencies of the selected GameObjects
        /// </summary>
        [MenuItem("GameObject/Select Dependencies", false, 11)]
        private static void SelectHierarchyDependencies()
        {
            Selection.objects = EditorUtility.CollectDependencies(Selection.gameObjects);

            Selection.objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        }
    }
}
