using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Varneon.EditorUtilities.ComponentExtensions
{
    /// <summary>
    /// Collection of context menu actions for LODGroups
    /// </summary>
    public static class LODGroupContextMenuActions
    {
        private static readonly string[] LODSuffixes = new string[]
        {
            "LOD0",
            "LOD1",
            "LOD2",
            "LOD3",
            "LOD4",
            "LOD5",
            "LOD6",
            "LOD7"
        };

        /// <summary>
        /// Gets all selected LODGroups.
        /// </summary>
        /// <returns>All selected LODGroups</returns>
        private static LODGroup[] GetSelectedLODGroups()
        {
            return Selection.gameObjects.Select(go => go.GetComponent<LODGroup>()).Where(l => l != null).ToArray();
        }

        #region Remove Missing Renderers
        /// <summary>
        /// Validate the method for removing missing renderer references from LODGroup
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/LODGroup/Remove Missing Renderers", validate = true)]
        private static bool ValidateRemoveMissingRenderers(MenuCommand command)
        {
            LODGroup lodGroup = (LODGroup)command.context;

            // Check if any of the renderer references is null
            return lodGroup.GetLODs().SelectMany(l => l.renderers).Any(r => r == null);
        }

        /// <summary>
        /// Remove missing renderer references from LODGroup
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/LODGroup/Remove Missing Renderers")]
        private static void RemoveMissingRenderers(MenuCommand command)
        {
            LODGroup lodGroup = (LODGroup)command.context;

            Undo.RecordObject(lodGroup, "Remove missing LODGroup renderers");

            lodGroup.SetLODs(lodGroup.GetLODs().Select(l => new LOD(l.screenRelativeTransitionHeight, l.renderers.Where(r => r != null).ToArray())).ToArray());
        }
        #endregion

        #region Append Unassigned LOD Renderers
        private struct LODGroupManifest
        {
            internal LODGroup LODGroup;

            internal Dictionary<int, Renderer[]> UnassingedRenderers;

            internal int LowestUnassignedLODLevel;
        }

        private static Dictionary<LODGroup, LODGroupManifest> lodGroupManifests;

        private static bool validationCooldownActive;

        [MenuItem("CONTEXT/LODGroup/Append Unassigned LOD Renderers (Experimental)", validate = true)]
        private static bool ValidateAppendUnassignedLODRenderers()
        {
            if (!validationCooldownActive)
            {
                lodGroupManifests = new Dictionary<LODGroup, LODGroupManifest>();

                validationCooldownActive = true;

                EditorApplication.delayCall += () => validationCooldownActive = false;

                return GetSelectedLODGroups().Count(l => CheckIfHasUnassignedLODRenderers(l)) > 0;
            }

            return lodGroupManifests.Count > 0;
        }

        [MenuItem("CONTEXT/LODGroup/Append Unassigned LOD Renderers (Experimental)")]
        private static void AppendUnassignedLODRenderers()
        {
            foreach(LODGroup lodGroup in lodGroupManifests.Keys)
            {
                AppendUnassignedLODRenderers(lodGroup);
            }
        }

        private static void AppendUnassignedLODRenderers(LODGroup lodGroup)
        {
            if (lodGroup == null) { return; }

            // Get all of the existing LODs
            List<LOD> lods = new List<LOD>(lodGroup.GetLODs());

            // Cache the original number of LOD levels
            int existingLODCount = lodGroup.lodCount;

            LODGroupManifest manifest = lodGroupManifests[lodGroup];

            // Cache the new LOD count
            int newLODCount = Mathf.Max(existingLODCount, manifest.LowestUnassignedLODLevel + 1);

            // Iterate the known maximum resulting number of LOD levels
            for (int i = 0; i < newLODCount; i++)
            {
                // Check if this LOD exceeds the existing number of LOD in the LODGroup
                bool countExceeded = i >= existingLODCount;

                // Create new LOD or get the existing one
                LOD lod = countExceeded ? new LOD() : lods[i];

                string suffix = LODSuffixes[i];

                // If the scan results indicate this LOD to not have missing renderers, continue to next one
                if (!manifest.UnassingedRenderers.ContainsKey(i))
                {
                    // If this LOD is exceeding the existing ones, add the empty LOD level to ensure consistency,
                    // otherwise e.g. LOD4 renderer could end up in LOD2 if there are no LOD2 and LOD3 renderers
                    if (countExceeded) { lods.Add(lod); }

                    continue;
                }

                // Get all found renderers with matching LOD suffix
                IEnumerable<Renderer> lodRenderers = manifest.UnassingedRenderers[i].Where(r => r.name.EndsWith(suffix));

                // If this LOD exceeds existing ones, add the new one to the list
                if (countExceeded)
                {
                    lod.renderers = lodRenderers.ToArray();

                    lods.Add(lod);
                }
                else // If this LOD already exists, union the existing renderers with found unassigned ones and override
                {
                    lod.renderers = lods[i].renderers.Union(lodRenderers).ToArray();

                    lods[i] = lod;
                }
            }

            // Iterate through the newly added LODs to ensure continuation of the transition heights
            for (int i = existingLODCount; i < newLODCount; i++)
            {
                LOD lod = lods[i];

                // Set the transition height to half of the previous LOD's height
                lod.screenRelativeTransitionHeight = lods[i - 1].screenRelativeTransitionHeight / 2f;

                lods[i] = lod;
            }

            Undo.RecordObject(lodGroup, "Append Unassigned LOD Renderers");

            // Apply the edited LODs back to the LODGroup
            lodGroup.SetLODs(lods.ToArray());
        }
        #endregion

        /// <summary>
        /// Check if LODGroup's hierarchy has any LOD renderers which have not been assigned to the LODGroup.
        /// </summary>
        /// <param name="lodGroup">LODGroup to validate</param>
        /// <returns>Has any of the child LOD Renderers not been assigned to the LODGroup.</returns>
        private static bool CheckIfHasUnassignedLODRenderers(LODGroup lodGroup)
        {
            Renderer[] foundRenderers = lodGroup.GetComponentsInChildren<Renderer>(true);

            LOD[] lods = lodGroup.GetLODs();

            Dictionary<int, Renderer[]> unassignedRenderers = new Dictionary<int, Renderer[]>();

            int lowestUnassignedLODLevel = 0;

            for (int i = 0; i < 8; i++)
            {
                string lodName = LODSuffixes[i];

                IEnumerable<Renderer> allLODRenderers = foundRenderers.Where(r => r.name.EndsWith(lodName));

                if(allLODRenderers.Count() == 0) { continue; }

                lowestUnassignedLODLevel = i;

                if (i >= lodGroup.lodCount)
                {
                    unassignedRenderers.Add(i, allLODRenderers.ToArray());
                }
                else
                {
                    Renderer[] lodRenderers = lods[i].renderers;

                    IEnumerable<Renderer> unassignedLODRenderers = allLODRenderers.Where(r => !lodRenderers.Contains(r));

                    if(unassignedLODRenderers.Count() == 0) { continue; }

                    unassignedRenderers.Add(i, unassignedLODRenderers.ToArray());
                }
            }

            bool containsUnassignedRenderers = unassignedRenderers.Count > 0;

            if (containsUnassignedRenderers)
            {
                LODGroupManifest manifest = new LODGroupManifest()
                {
                    LODGroup = lodGroup,
                    UnassingedRenderers = unassignedRenderers,
                    LowestUnassignedLODLevel = lowestUnassignedLODLevel
                };

                lodGroupManifests.Add(lodGroup, manifest);
            }

            return containsUnassignedRenderers;
        }

        #region Select Renderers At LOD
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/0", validate = true)] private static bool ValidateSelectLODRenderers0() => ValidateSelectedLODGroupsHaveEnoughLevels(1);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/1", validate = true)] private static bool ValidateSelectLODRenderers1() => ValidateSelectedLODGroupsHaveEnoughLevels(2);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/2", validate = true)] private static bool ValidateSelectLODRenderers2() => ValidateSelectedLODGroupsHaveEnoughLevels(3);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/3", validate = true)] private static bool ValidateSelectLODRenderers3() => ValidateSelectedLODGroupsHaveEnoughLevels(4);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/4", validate = true)] private static bool ValidateSelectLODRenderers4() => ValidateSelectedLODGroupsHaveEnoughLevels(5);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/5", validate = true)] private static bool ValidateSelectLODRenderers5() => ValidateSelectedLODGroupsHaveEnoughLevels(6);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/6", validate = true)] private static bool ValidateSelectLODRenderers6() => ValidateSelectedLODGroupsHaveEnoughLevels(7);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/7", validate = true)] private static bool ValidateSelectLODRenderers7() => ValidateSelectedLODGroupsHaveEnoughLevels(8);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/0")] private static void SelectLODRenderers0() => SelectLODRenderers(0);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/1")] private static void SelectLODRenderers1() => SelectLODRenderers(1);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/2")] private static void SelectLODRenderers2() => SelectLODRenderers(2);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/3")] private static void SelectLODRenderers3() => SelectLODRenderers(3);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/4")] private static void SelectLODRenderers4() => SelectLODRenderers(4);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/5")] private static void SelectLODRenderers5() => SelectLODRenderers(5);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/6")] private static void SelectLODRenderers6() => SelectLODRenderers(6);
        [MenuItem("CONTEXT/LODGroup/Select Renderers At LOD/7")] private static void SelectLODRenderers7() => SelectLODRenderers(7);

        /// <summary>
        /// Ensure that an LODGroup has a minimum number of LOD levels.
        /// </summary>
        /// <param name="minLevels">Minimum number of LOD levels to satisfy this check.</param>
        /// <returns>Does the LODGroup have enough LOD levels.</returns>
        private static bool ValidateSelectedLODGroupsHaveEnoughLevels(int minLevels)
        {
            return GetSelectedLODGroups().Min(l => l.lodCount) >= minLevels;
        }

        /// <summary>
        /// Is the cooldown for selecting Renderers on LODGroup at certain LOD level active
        /// </summary>
        private static bool lodRendererSelectionCooldownActive;

        /// <summary>
        /// Selects all Renderers on specified LOD level across all selected LODGroups.
        /// </summary>
        /// <param name="level">LOD level to select Renderers from</param>
        private static void SelectLODRenderers(int level)
        {
            // Check if multiple calls are received on the same frame
            if (lodRendererSelectionCooldownActive) { return; }

            Selection.objects = Selection.gameObjects.SelectMany(g => g.GetComponents<LODGroup>())
                .SelectMany(l => l.GetLODs()[level].renderers)
                .Where(r => r != null).Select(r => r.gameObject).ToArray();

            // Enable cooldown for this method so following calls received on same frame won't invalidate the selection
            lodRendererSelectionCooldownActive = true;

            // Disable cooldown for this method on the next update
            EditorApplication.delayCall += () => lodRendererSelectionCooldownActive = false;
        }
        #endregion
    }
}
