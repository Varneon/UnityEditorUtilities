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

        #region Duplicate Component
        private static readonly Type[] SingleInstanceNativeComponents = new Type[]
        {
            typeof(Transform),
            #region LAYOUT
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasGroup),
            #endregion
            #region MESH
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(SkinnedMeshRenderer),
            #endregion
            #region MISCELLANEOUS
            typeof(Animation),
            typeof(Animator),
            typeof(BillboardRenderer),
            typeof(Grid),
            typeof(ParticleSystemForceField),
            typeof(SpriteMask),
            typeof(UnityEngine.U2D.SpriteShapeRenderer),
            typeof(Terrain),
            typeof(WindZone),
            #endregion
            #region NAVIGATION
            typeof(UnityEngine.AI.NavMeshAgent),
            typeof(UnityEngine.AI.NavMeshObstacle),
            #endregion
            #region PHYSICS
            #if UNITY_2020_1_OR_NEWER
            typeof(ArticulationBody),
            #endif
            typeof(CharacterController),
            typeof(Cloth),
            typeof(ConstantForce),
            typeof(Rigidbody),
            typeof(TerrainCollider),
            typeof(WheelCollider),
            #endregion
            #region PLAYABLES
            typeof(UnityEngine.Playables.PlayableDirector),
            #endregion
            #region RENDERING
            typeof(Camera),
            typeof(CanvasRenderer),
            typeof(FlareLayer),
            typeof(LODGroup),
            typeof(Light),
            typeof(LightProbeGroup),
            typeof(LightProbeProxyVolume),
            typeof(OcclusionArea),
            typeof(OcclusionPortal),
            typeof(ReflectionProbe),
            typeof(UnityEngine.Rendering.SortingGroup),
            typeof(SpriteRenderer),
            typeof(StreamingController),
            #endregion
            #region TILEMAP
            typeof(UnityEngine.Tilemaps.Tilemap),
            typeof(UnityEngine.Tilemaps.TilemapCollider2D),
            typeof(UnityEngine.Tilemaps.TilemapRenderer),
            #endregion
            #region AUDIO
            typeof(AudioChorusFilter),
            typeof(AudioDistortionFilter),
            typeof(AudioEchoFilter),
            typeof(AudioHighPassFilter),
            typeof(AudioListener),
            typeof(AudioLowPassFilter),
            typeof(AudioReverbFilter),
            typeof(AudioReverbZone),
            #endregion
            #region VISUAL EFFECTS
            //typeof(Halo)
            typeof(LensFlare),
            typeof(LineRenderer),
            typeof(ParticleSystem),
            typeof(Projector),
            typeof(TrailRenderer),
            typeof(UnityEngine.VFX.VisualEffect)
            #endregion
        };

        /// <summary>
        /// Ensure that the user isn't trying to duplicate a Component that only allows a single instance per GameObject
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [MenuItem("CONTEXT/Component/Duplicate Component", true, 500)]
        private static bool ValidateDuplicateComponent(MenuCommand command)
        {
            Type type = command.context.GetType();

            return !Attribute.IsDefined(command.context.GetType(), typeof(DisallowMultipleComponent)) && !SingleInstanceNativeComponents.Contains(type);
        }

        /// <summary>
        /// Shortcut menu item for copying and pasting a Component
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Component/Duplicate Component", false, 500)]
        private static void DuplicateComponent(MenuCommand command)
        {
            try
            {
                Component c = command.context as Component;

                Undo.RegisterCompleteObjectUndo(c.gameObject, "Duplicate Component");

                ComponentUtility.CopyComponent(c);

                if (!ComponentUtility.PasteComponentAsNew(c.gameObject))
                {
                    Debug.LogWarning("Failed to duplicate the component!");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion
    }
}
