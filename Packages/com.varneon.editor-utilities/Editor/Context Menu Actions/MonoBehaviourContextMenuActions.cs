using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for MonoBehaviours
    /// </summary>
    public static class MonoBehaviourContextMenuActions
    {
        /// <summary>
        /// Selects the source script of a MonoBehaviour
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/MonoBehaviour/Select Script")]
        private static void SelectScript(MenuCommand command)
        {
            try
            {
                MonoBehaviour monoBehaviour = command.context as MonoBehaviour;

                Assert.IsNotNull(monoBehaviour, "MonoBehaviour of the context Component is not valid.");

                EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(monoBehaviour));
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
