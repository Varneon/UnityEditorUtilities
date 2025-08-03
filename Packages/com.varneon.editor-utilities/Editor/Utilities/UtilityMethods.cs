using System;
using System.Reflection;
using UnityEngine;

namespace Varneon.EditorUtilities
{
    /// <summary>
    /// Generic utility methods
    /// </summary>
    public static class UtilityMethods
    {
        /// <summary>
        /// Check if a Component is required on a GameObject by another Component
        /// </summary>
        /// <param name="gameObject">GameObject on which the Component may be required</param>
        /// <param name="component">Component which may be required on the GameObject</param>
        /// <param name="requiringComponent">Component that requires the Component being checked to be present. Null if not required.</param>
        /// <returns>True if <paramref name="component"/> is required on <paramref name="gameObject"/> by <paramref name="requiringComponent"/>.</returns>
        public static bool IsComponentRequired(GameObject gameObject, Component component, out Component requiringComponent)
        {
            Type type = component.GetType();

            foreach(Component c in gameObject.GetComponents<Component>())
            {
                foreach(RequireComponent a in c.GetType().GetCustomAttributes<RequireComponent>(true))
                {
                    if ((a.m_Type0?.IsAssignableFrom(type) ?? false) ||
                        (a.m_Type1?.IsAssignableFrom(type) ?? false) ||
                        (a.m_Type2?.IsAssignableFrom(type) ?? false))
                    {
                        requiringComponent = c;

                        return true;
                    }
                }
            }

            requiringComponent = null;

            return false;
        }
    }
}
