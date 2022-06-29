using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for Joints
    /// </summary>
    public class JointContextMenuActions : Editor
    {
        /// <summary>
        /// Validate the method for setting the joint's connected body to the first rigidbody that can be found in the transform's parents
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Joint/Set Connected Body To Parent", validate = true)]
        private static bool ValidateSetJointConnectedBodyToParentRigidbody(MenuCommand command)
        {
            return ((Joint)command.context).transform.parent != null;
        }

        /// <summary>
        /// Sets the joint's connected body to the first rigidbody that can be found in the transform's parents
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Joint/Set Connected Body To Parent")]
        private static void SetJointConnectedBodyToParentRigidbody(MenuCommand command)
        {
            Joint joint = (Joint)command.context;

            Transform parent = joint.transform.parent;

            if (parent == null) { Debug.LogWarning("Joint doesn't have a parent!", joint); return; }

            Rigidbody parentRigidbody = parent.GetComponentInParent<Rigidbody>();

            if (parentRigidbody == null) { Debug.LogWarning("Couldn't find Rigidbody from any parent!", joint); return; }

            Undo.RecordObject(joint, "Assign Joint Connected Body From Parent");

            joint.connectedBody = parentRigidbody;
        }

        /// <summary>
        /// Validate the method for setting the joint's connected body to the first rigidbody that can be found in the transform's parents
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Joint/Set Connected Body To Child", validate = true)]
        private static bool ValidateSetJointConnectedBodyToChildRigidbody(MenuCommand command)
        {
            return ((Joint)command.context).transform.childCount > 0;
        }

        /// <summary>
        /// Sets the joint's connected body to the first rigidbody that can be found in the transform's children
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Joint/Set Connected Body To Child")]
        private static void SetJointConnectedBodyToChildRigidbody(MenuCommand command)
        {
            Joint joint = (Joint)command.context;

            if (joint.transform.childCount == 0) { Debug.LogWarning("Joint doesn't have any children!", joint); return; }

            Rigidbody childRigidbody = joint.transform.GetComponentsInChildren<Rigidbody>().FirstOrDefault(r => r.transform != joint.transform);

            if (childRigidbody == null) { Debug.LogWarning("Couldn't find Rigidbody from any child!", joint); return; }

            Undo.RecordObject(joint, "Assign Joint Connected Body From Child");

            joint.connectedBody = childRigidbody;
        }
    }
}
