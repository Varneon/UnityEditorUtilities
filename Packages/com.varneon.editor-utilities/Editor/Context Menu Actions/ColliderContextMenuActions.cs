using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for <see cref="Collider"/>s
    /// </summary>
    public static class ColliderContextMenuActions
    {
        [MenuItem("CONTEXT/BoxCollider/Recalculate Bounds/Renderers (Mesh + Skinned)")]
        private static void RecalculateBounds(MenuCommand cmd)
        {
            BoxCollider collider = cmd.context as BoxCollider;

            Undo.RecordObject(collider, "Recalculate BoxCollider Bounds");

            Bounds bounds = BoundsUtility.CalculateRendererBounds(collider.transform, true);

            collider.center = bounds.center;
            collider.size = bounds.size;
        }
    }
}
