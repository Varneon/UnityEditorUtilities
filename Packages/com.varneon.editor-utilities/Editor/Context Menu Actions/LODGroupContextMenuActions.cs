using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for LODGroups
    /// </summary>
    public static class LODGroupContextMenuActions
    {
        /// <summary>
        /// Validate the method for removing missing renderer references from LODGroup
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/LODGroup/Remove Missing Renderers", validate = true)]
        private static bool ValidateRemoveMissingRenderers(MenuCommand command)
        {
            LODGroup lodGroup = (LODGroup)command.context;

            // Check if any of the renderer references is null
            return lodGroup.GetLODs().SelectMany(l => l.renderers).Any(r => r == null);
        }

        /// <summary>
        /// Remove missing renderer references from LODGroup
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/LODGroup/Remove Missing Renderers")]
        private static void RemoveMissingRenderers(MenuCommand command)
        {
            LODGroup lodGroup = (LODGroup)command.context;

            Undo.RecordObject(lodGroup, "Remove missing LODGroup renderers");

            lodGroup.SetLODs(lodGroup.GetLODs().Select(l => new LOD(l.screenRelativeTransitionHeight, l.renderers.Where(r => r != null).ToArray())).ToArray());
        }

        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/0", validate = true)] private static bool ValidateSelectLODRenderers0() => ValidateLODGroupHasEnoughLevels(1);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/1", validate = true)] private static bool ValidateSelectLODRenderers1() => ValidateLODGroupHasEnoughLevels(2);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/2", validate = true)] private static bool ValidateSelectLODRenderers2() => ValidateLODGroupHasEnoughLevels(3);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/3", validate = true)] private static bool ValidateSelectLODRenderers3() => ValidateLODGroupHasEnoughLevels(4);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/4", validate = true)] private static bool ValidateSelectLODRenderers4() => ValidateLODGroupHasEnoughLevels(5);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/5", validate = true)] private static bool ValidateSelectLODRenderers5() => ValidateLODGroupHasEnoughLevels(6);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/6", validate = true)] private static bool ValidateSelectLODRenderers6() => ValidateLODGroupHasEnoughLevels(7);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/7", validate = true)] private static bool ValidateSelectLODRenderers7() => ValidateLODGroupHasEnoughLevels(8);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/0")] private static void SelectLODRenderers0() => SelectLODRenderers(0);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/1")] private static void SelectLODRenderers1() => SelectLODRenderers(1);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/2")] private static void SelectLODRenderers2() => SelectLODRenderers(2);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/3")] private static void SelectLODRenderers3() => SelectLODRenderers(3);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/4")] private static void SelectLODRenderers4() => SelectLODRenderers(4);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/5")] private static void SelectLODRenderers5() => SelectLODRenderers(5);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/6")] private static void SelectLODRenderers6() => SelectLODRenderers(6);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/7")] private static void SelectLODRenderers7() => SelectLODRenderers(7);

        /// <summary>
        /// Ensure that an LODGroup has a minimum number of LOD levels.
        /// </summary>
        /// <param name="minLevels">Minimum number of LOD levels to satisfy this check.</param>
        /// <returns>Does the LODGroup have enough LOD levels.</returns>
        private static bool ValidateLODGroupHasEnoughLevels(int minLevels)
        {
            return Selection.gameObjects.SelectMany(g => g.GetComponents<LODGroup>()).Min(l => l.lodCount) >= minLevels;
        }

        /// <summary>
        /// Is the cooldown for selecting Renderers on LODGroup at certain LOD level active
        /// </summary>
        private static bool lodRendererSelectionCooldownActive;

        /// <summary>
        /// Selects all Renderers on specified LOD level across all selected LODGroups.
        /// </summary>
        /// <param name="level">LOD level to select Renderers from</param>
        private static void SelectLODRenderers(int level)
        {
            // Check if multiple calls are received on the same frame
            if (lodRendererSelectionCooldownActive) { return; }

            Selection.objects = Selection.gameObjects.SelectMany(g => g.GetComponents<LODGroup>())
                .SelectMany(l => l.GetLODs()[level].renderers)
                .Where(r => r != null).Select(r => r.gameObject).ToArray();

            // Enable cooldown for this method so following calls received on same frame won't invalidate the selection
            lodRendererSelectionCooldownActive = true;

            // Disable cooldown for this method on the next update
            EditorApplication.delayCall += () => lodRendererSelectionCooldownActive = false;
        }
    }
}
