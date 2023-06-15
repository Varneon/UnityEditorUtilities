using UnityEditor;
using UnityEngine;

namespace Varneon.EditorUtilities.HierarchyActions
{
    /// <summary>
    /// Collection of hierarchy context menu actions for creating proxy objects
    /// </summary>
    public static class ProxyObjectActions
    {
        // 'Create Empty Parent' was introduced in 2020.2
        // https://docs.unity3d.com/2020.2/Documentation/Manual/Hierarchy.html
#if !UNITY_2020_2_OR_NEWER
        /// <summary>
        /// Creates a new proxy parent object for the selected object(s)
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("GameObject/Create Parent Proxy", false, 0)]
        private static void CreateProxyParent(MenuCommand command)
        {
            // Get the selected object's transform
            Transform contextTransform = ((GameObject)command.context).transform;

            // Create a new object
            Transform newProxyTransform = new GameObject(string.Format("{0}_Proxy", contextTransform.name)).transform;

            // Match the proxy's position and rotation to the original's
            newProxyTransform.SetPositionAndRotation(contextTransform.position, contextTransform.rotation);
            
            // Match the parent
            newProxyTransform.SetParent(contextTransform.parent);

            // Match the sibling index
            newProxyTransform.SetSiblingIndex(contextTransform.GetSiblingIndex());

            // Match the local scale
            newProxyTransform.localScale = contextTransform.localScale;

            // Register undo for the new proxy
            Undo.RegisterCreatedObjectUndo(newProxyTransform.gameObject, "Create proxy parent");

            // Parent the original object to the new proxy with undo
            Undo.SetTransformParent(contextTransform, newProxyTransform, "Parent object to proxy");
        }
#endif
    }
}

