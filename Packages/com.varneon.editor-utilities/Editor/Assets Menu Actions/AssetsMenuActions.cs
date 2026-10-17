using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Varneon.EditorUtilities.AssetsMenuActions
{
    public static class AssetsMenuActions
    {
        [MenuItem("Assets/Copy Full Path(s)", false, 19)]
        private static void CopyFullPath()
        {
            if(Selection.count > 1)
            {
                string[] paths = Selection.objects.Select(o => Path.GetFullPath(AssetDatabase.GetAssetPath(o))).Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p)).ToArray();

                EditorGUIUtility.systemCopyBuffer = string.Join(Environment.NewLine, paths.Select(p => $"\"{p}\""));
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (!string.IsNullOrEmpty(path))
                {
                    EditorGUIUtility.systemCopyBuffer = File.Exists(path) ? Path.GetFullPath(path) : path;
                }
            }
        }

        [MenuItem("Assets/Copy GUID", false, 19)]
        private static void CopyGUID()
        {
            if(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out string guid, out long _))
            {
                EditorGUIUtility.systemCopyBuffer = guid;
            }
        }
    }
}
