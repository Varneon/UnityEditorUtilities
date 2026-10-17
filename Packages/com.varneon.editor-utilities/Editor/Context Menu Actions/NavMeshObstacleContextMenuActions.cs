using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for <see cref="NavMeshObstacle"/>s
    /// </summary>
    internal static class NavMeshObstacleContextMenuActions
    {
        private const string
            PATH_ROOT = "CONTEXT/NavMeshObstacle/Recalculate Bounds",
            PATH_RENDERERS = PATH_ROOT + "/Renderers (Mesh + Skinned)",
            PATH_COLLIDERS = PATH_ROOT + "/Colliders (Non-Trigger)";

        [MenuItem(PATH_RENDERERS, validate = true)]
        [MenuItem(PATH_COLLIDERS, validate = true)]
        private static bool ValidateNavMeshObstacleBoxMode(MenuCommand cmd)
        {
            NavMeshObstacle obstacle = cmd.context as NavMeshObstacle;

            return obstacle.shape == NavMeshObstacleShape.Box;
        }

        [MenuItem(PATH_RENDERERS)]
        private static void RecalculateBoundsRenderers(MenuCommand cmd)
        {
            NavMeshObstacle obstacle = cmd.context as NavMeshObstacle;

            Undo.RecordObject(obstacle, "Recalculate NavMeshObstacle Bounds");

            Bounds bounds = BoundsUtility.CalculateRendererBounds(obstacle.transform, true);

            obstacle.center = bounds.center;
            obstacle.size = bounds.size;
        }

        [MenuItem(PATH_COLLIDERS)]
        private static void RecalculateBoundsColliders(MenuCommand cmd)
        {
            NavMeshObstacle obstacle = cmd.context as NavMeshObstacle;

            Undo.RecordObject(obstacle, "Recalculate NavMeshObstacle Bounds");

            Bounds bounds = BoundsUtility.CalculateHierarchyColliderBounds(obstacle.transform, true);

            obstacle.center = bounds.center;
            obstacle.size = bounds.size;
        }
    }
}
