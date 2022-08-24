using System.IO;
using UnityEditor;

namespace Varneon.EditorUtilities.AssetsMenuActions
{
    public static class AssetsMenuActions
    {
        [MenuItem("Assets/Copy Full Path", false, 19)]
        private static void CopyFullPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!string.IsNullOrEmpty(path))
            {
                EditorGUIUtility.systemCopyBuffer = File.Exists(path) ? Path.GetFullPath(path) : path;
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
