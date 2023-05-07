using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.SearchableEditorWindow;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for Components
    /// </summary>
    public static class ComponentContextMenuActions
    {
        /// <summary>
        /// Type of the editor's SceneHierarchyWindow
        /// </summary>
        private static readonly Type SceneHierarchyWindowType = Type.GetType("UnityEditor.SceneHierarchyWindow, UnityEditor");

        /// <summary>
        /// Internal method for setting the search filter of the hierarchy
        /// </summary>
        private static readonly MethodInfo SetSearchFilterMethod = SceneHierarchyWindowType.GetMethod("SetSearchFilter", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Finds all components of type in scene
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Component/Find Components Of Type In Scene")]
        private static void FindComponentsOfTypeInScene(MenuCommand command)
        {
            try
            {
                // Get the component of the current context
                Component component = command.context as Component;

                // Ensure that the context component is not null
                if(component == null) { Debug.LogWarning("Context Component is not valid!"); return; }

                // Find the hierarchy window
                object hierarchyWindow = Resources.FindObjectsOfTypeAll(SceneHierarchyWindowType).FirstOrDefault();

                // Ensure that the window is not null
                if (hierarchyWindow == null) { Debug.LogWarning("Couldn't find scene hierarchy window!"); return; }

                // Invoke the search filter set method via reflection
                SetSearchFilterMethod.Invoke(hierarchyWindow, new object[] { $"t:{component.GetType().Name}", SearchMode.All, true, false});
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
