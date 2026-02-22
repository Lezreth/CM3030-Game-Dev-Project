using UnityEngine;
using UnityEditor;

public class BulkTextureResize
{
    [MenuItem("Tools/Resize All Textures To 512 (WebGL)")]
    static void ResizeTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                {
                    name = "WebGL",
                    overridden = true,
                    maxTextureSize = 512,
                    textureCompression = TextureImporterCompression.Compressed
                });

                importer.SaveAndReimport();
            }
        }

        Debug.Log("All textures resized to 512 for WebGL.");
    }
}