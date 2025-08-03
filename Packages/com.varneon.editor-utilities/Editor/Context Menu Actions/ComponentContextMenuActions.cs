using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.SearchableEditorWindow;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for Components
    /// </summary>
    public static class ComponentContextMenuActions
    {
        #region Find Components Of Type In Scene
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
        #endregion

        #region Cut Component
        /// <summary>
        /// Ensure that the user isn't trying to cut a Transform component which is required at all times
        /// </summary>
        [MenuItem("CONTEXT/Component/Cut Component", true, 500)]
        private static bool ValidateCutComponent(MenuCommand command)
        {
            return command.context.GetType() != typeof(Transform);
        }

        /// <summary>
        /// Shortcut menu item for copying and removing a Component
        /// </summary>
        [MenuItem("CONTEXT/Component/Cut Component", false, 500)]
        private static void CutComponent(MenuCommand command)
        {
            Component c = command.context as Component;

            ComponentUtility.CopyComponent(c);

            if(UtilityMethods.IsComponentRequired(c.gameObject, c, out Component rc))
            {
                EditorUtility.DisplayDialog("Can't remove component", string.Format("Can't remove {0} because {1} depends on it", c.GetType().Name, rc.GetType().Name), "Ok");
            }
            else
            {
                Undo.DestroyObjectImmediate(c);
            }
        }
        #endregion
    }
}
