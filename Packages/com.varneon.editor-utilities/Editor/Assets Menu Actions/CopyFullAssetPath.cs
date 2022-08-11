using System.IO;
using UnityEditor;

namespace Varneon.EditorUtilities.AssetsMenuActions
{
    public class CopyFullAssetPath : Editor
    {
        [MenuItem("Assets/Copy Full Path", false, 19)]
        private static void CopyFullPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                EditorGUIUtility.systemCopyBuffer = Path.GetFullPath(path);
            }
        }
    }
}
