using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Varneon.EditorUtilities
{
    /// <summary>
    /// Utility for <see cref="Bounds"/>
    /// </summary>
    public class BoundsUtility
    {
        /// <summary>
        /// Calculates the local bounds of all Renderers in a hierarchy relative to the root
        /// </summary>
        /// <param name="root">Root transform of the hierarchy</param>
        /// <param name="ignoreWithoutMaterials">Should Renderers without materials be ignored from bounds</param>
        /// <returns>Local bounds of all Renderers in the hierarchy</returns>
        [PublicAPI]
        public static Bounds CalculateRendererBounds(Transform root, bool ignoreWithoutMaterials = false) => CalculateRendererBounds(root, Physics.AllLayers, ignoreWithoutMaterials);

        /// <summary>
        /// Calculates the local bounds of all Renderers in a hierarchy relative to the root, limited to specified layers
        /// </summary>
        /// <param name="root">Root transform of the hierarchy</param>
        /// <param name="layers">Layers to filter the calculated Renderers</param>
        /// <param name="ignoreWithoutMaterials">Should Renderers without materials be ignored from bounds</param>
        /// <returns>Local bounds of all Renderers in the hierarchy</returns>
        [PublicAPI]
        public static Bounds CalculateRendererBounds(Transform root, LayerMask layers, bool ignoreWithoutMaterials = false)
        {
            if (root == null) { throw new ArgumentNullException(nameof(root)); }

            bool initialized = false;

            Bounds bounds = default;

            Matrix4x4 worldToRootMatrix = root.worldToLocalMatrix;

            foreach (MeshRenderer renderer in root.GetComponentsInChildren<MeshRenderer>())
            {
                if (!IsLayerDefined(layers, renderer.gameObject.layer)) { continue; }

                if (ignoreWithoutMaterials && renderer.sharedMaterials.Length == 0) { continue; }

                Bounds localBounds = renderer.localBounds;

                Transform rendererTransform = renderer.transform;

                Matrix4x4 rendererToWorldMatrix = rendererTransform.localToWorldMatrix;

                if (initialized)
                {
                    EncapsulatePoints(ref bounds, GetBoundsCorners(localBounds, rendererToWorldMatrix, worldToRootMatrix));
                }
                else
                {
                    bounds = GetPointsBounds(GetBoundsCorners(localBounds, rendererToWorldMatrix, worldToRootMatrix));

                    initialized = true;
                }
            }

            foreach (SkinnedMeshRenderer renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (!IsLayerDefined(layers, renderer.gameObject.layer)) { continue; }

                if (ignoreWithoutMaterials && renderer.sharedMaterials.Length == 0) { continue; }

                Bounds localBounds = renderer.localBounds;

                Transform rendererTransform = renderer.transform;

                Matrix4x4 rendererToWorldMatrix = rendererTransform.localToWorldMatrix;

                if (initialized)
                {
                    EncapsulatePoints(ref bounds, GetBoundsCorners(localBounds, rendererToWorldMatrix, worldToRootMatrix));
                }
                else
                {
                    bounds = GetPointsBounds(GetBoundsCorners(localBounds, rendererToWorldMatrix, worldToRootMatrix));

                    initialized = true;
                }
            }

            return bounds;
        }

        /// <summary>
        /// Calculates the local bounds of all Colliders in a hierarchy relative to the root
        /// </summary>
        /// <param name="root">Root transform of the hierarchy</param>
        /// <param name="ignoreTriggers">Should Colliders set to Trigger be ignored from bounds</param>
        /// <returns>Local bounds of all Colliders in the hierarchy</returns>
        [PublicAPI]
        public static Bounds CalculateHierarchyColliderBounds(Transform root, bool ignoreTriggers = true) => CalculateHierarchyColliderBounds(root, Physics.AllLayers, ignoreTriggers);

        /// <summary>
        /// Calculates the local bounds of all Colliders in a hierarchy relative to the root, limited to specified layers
        /// </summary>
        /// <param name="root">Root transform of the hierarchy</param>
        /// <param name="layers">Layers to filter the calculated Colliders</param>
        /// <param name="ignoreTriggers">Should Colliders set to Trigger be ignored from bounds</param>
        /// <returns>Local bounds of all Colliders in the hierarchy</returns>
        [PublicAPI]
        public static Bounds CalculateHierarchyColliderBounds(Transform root, LayerMask layers, bool ignoreTriggers = true)
        {
            if (root == null) { throw new ArgumentNullException(nameof(root)); }

            Bounds bounds = new Bounds();

            foreach (Collider collider in root.GetComponentsInChildren<Collider>())
            {
                if (!IsLayerDefined(layers, collider.gameObject.layer)) { continue; }

                if (ignoreTriggers && collider.isTrigger) { continue; }

                Type colliderType = collider.GetType();

                if (colliderType.Equals(typeof(BoxCollider)))
                {
                    BoxCollider boxCollider = (BoxCollider)collider;

                    bounds.Encapsulate(new Bounds(boxCollider.center, boxCollider.size));
                }
                else if (colliderType.Equals(typeof(CapsuleCollider)))
                {
                    CapsuleCollider capsuleCollider = (CapsuleCollider)collider;

                    float height = capsuleCollider.height;

                    float radius = capsuleCollider.radius;

                    Vector3 size = new Vector3(
                        capsuleCollider.direction == 0 ? height : radius,
                        capsuleCollider.direction == 1 ? height : radius,
                        capsuleCollider.direction == 2 ? height : radius
                        );

                    bounds.Encapsulate(new Bounds(capsuleCollider.center, size));
                }
                else if (colliderType.Equals(typeof(SphereCollider)))
                {
                    SphereCollider sphereCollider = (SphereCollider)collider;

                    float radius = sphereCollider.radius;

                    bounds.Encapsulate(new Bounds(sphereCollider.center, new Vector3(radius, radius, radius)));
                }
                else if (colliderType.Equals(typeof(MeshCollider)))
                {
                    MeshCollider meshCollider = (MeshCollider)collider;

                    bounds.Encapsulate(meshCollider.sharedMesh.bounds);
                }
            }
            return bounds;
        }

        /// <summary>
        /// Creates new Bounds that encapsulates all provided points
        /// </summary>
        /// <param name="points">Positions to be encapsulated by the Bounds</param>
        /// <returns>Bounds encapsulating all provided points</returns>
        [PublicAPI]
        public static Bounds GetPointsBounds(params Vector3[] points)
        {
            Bounds bounds = new Bounds(points[0], Vector3.zero);

            EncapsulatePoints(ref bounds, points);

            return bounds;
        }

        /// <summary>
        /// Encapsulates an array of points inside Bounds
        /// </summary>
        /// <param name="bounds">Bounds to encapsulate the points with</param>
        /// <param name="points">Points to encapsulate with the Bounds</param>
        [PublicAPI]
        public static void EncapsulatePoints(ref Bounds bounds, params Vector3[] points)
        {
            foreach (Vector3 point in points)
            {
                bounds.Encapsulate(point);
            }
        }

        /// <summary>
        /// Gets all eight corners of Bounds as Vector3 array, transformed from Bounds' local space to defined root space.
        /// <para>The corners are first transformed from the local space of the object the Bounds are from into world space, then transformed to the desired root's local space for evaluation</para>
        /// </summary>
        /// <param name="bounds">Bounds to extract the corners from</param>
        /// <param name="boundsToWorldMatrix">Matrix4x4 to transform from Bounds' local space to world, e.g. <see cref="Transform.localToWorldMatrix"/></param>
        /// <param name="worldToRootMatrix">Matrix4x4 to transform the world-space bounds to root space, e.g. <see cref="Transform.worldToLocalMatrix"/></param>
        /// <returns>All corners of the Bounds, transformed by provided Matrices</returns>
        [PublicAPI]
        public static Vector3[] GetBoundsCorners(Bounds bounds, Matrix4x4 boundsToWorldMatrix, Matrix4x4 worldToRootMatrix)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;

            float x = e.x;
            float y = e.y;
            float z = e.z;
            float xn = -x;
            float yn = -y;
            float zn = -z;

            return new Vector3[]
            {
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + e)),
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + new Vector3(xn, y, z))),
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + new Vector3(xn, yn, z))),
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + new Vector3(xn, yn, zn))),
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + new Vector3(x, yn, zn))),
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + new Vector3(x, y, zn))),
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + new Vector3(xn, y, zn))),
                worldToRootMatrix.MultiplyPoint(boundsToWorldMatrix.MultiplyPoint(c + new Vector3(x, yn, z)))
            };
        }

        /// <summary>
        /// Gets all eight corners of Bounds as Vector3 array, multiplied by a Matrix4x4
        /// </summary>
        /// <param name="bounds">Bounds to extract the corners from</param>
        /// <param name="matrix">Matrix4x4 to multiply all the corner positions with</param>
        /// <returns>All corners of the Bounds, transformed by provided Matrix</returns>
        [PublicAPI]
        public static Vector3[] GetBoundsCorners(Bounds bounds, Matrix4x4 matrix)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;

            float x = e.x;
            float y = e.y;
            float z = e.z;
            float xn = -x;
            float yn = -y;
            float zn = -z;

            return new Vector3[]
            {
                matrix.MultiplyPoint(c + e),
                matrix.MultiplyPoint(c + new Vector3(xn, y, z)),
                matrix.MultiplyPoint(c + new Vector3(xn, yn, z)),
                matrix.MultiplyPoint(c + new Vector3(xn, yn, zn)),
                matrix.MultiplyPoint(c + new Vector3(x, yn, zn)),
                matrix.MultiplyPoint(c + new Vector3(x, y, zn)),
                matrix.MultiplyPoint(c + new Vector3(xn, y, zn)),
                matrix.MultiplyPoint(c + new Vector3(x, yn, z))
            };
        }

        private static bool IsLayerDefined(LayerMask mask, int layer) => (mask & (1 << layer)) != 0;
    }
}
