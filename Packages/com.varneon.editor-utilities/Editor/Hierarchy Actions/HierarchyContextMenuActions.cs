using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

        /// <summary>
        /// Selects all found components of type in the hierarchy
        /// </summary>
        [MenuItem("GameObject/Select All Components Of Type", false, 11)]
        private static void SelectAllHierarchyComponentsOfType()
        {
            EditorApplication.hierarchyWindowItemOnGUI += ShowTypeSelectionDropdown;

            void ShowTypeSelectionDropdown(int instanceID, Rect selectionRect)
            {
                // Find all component types in the selected hierarchy
                HashSet<Type> types = new HashSet<Type>(Selection.gameObjects.SelectMany(go => go.GetComponentsInChildren<Component>(true)).Select(c => c.GetType()).OrderBy(t => t.Name));

                GenericMenu menu = new GenericMenu();

                // Add new menu items for each component type
                for (int i = 0; i < types.Count(); i++)
                {
                    menu.AddItem(new GUIContent(types.ElementAt(i).Name), false, SelectAllComponentsOfType, types.ElementAt(i));
                }

                // GenericMenu method for selecting a component type
                void SelectAllComponentsOfType(object userData)
                {
                    Type componentType = userData as Type;

                    // Get all GameObjects with the selected type of component attached to it
                    GameObject[] gameObjects = Selection.gameObjects.SelectMany(go => go.GetComponentsInChildren<Transform>(true)).Select(t => t.gameObject).Where(t => t.GetComponent(componentType) != null).ToArray();

                    Selection.objects = gameObjects;
                }

                // Show the menu
                menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));

                EditorApplication.hierarchyWindowItemOnGUI -= ShowTypeSelectionDropdown;
            }
        }
    }
}
