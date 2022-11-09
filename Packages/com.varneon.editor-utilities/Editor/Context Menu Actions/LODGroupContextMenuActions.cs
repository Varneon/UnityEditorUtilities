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
    }
}
