using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeEditor;
    Editor colourEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if(check.changed) 
            {
                planet.GeneratePlanet();
            }
        }

        if(GUILayout.Button("GeneratePlanet")) 
        {
            planet.GeneratePlanet();
        }

        if(GUILayout.Button("Save Planet"))
        {
            // string filePath = "Assets/" + planet.planetName + "/" + planet.planetName; 
            // planet.GeneratePlanet();
            // byte[] bytes = planet.colourGenerator.EncodeTextureToPNG();

            // var gameObj = new GameObject(planet.planetName);
            // var newPlanet = gameObj.AddComponent<Planet>();

            // newPlanet = planet.GetComponent<Planet>();

            // AssetDatabase.CreateFolder("Assets", planet.planetName);
            // AssetDatabase.CreateAsset(new Material(planet.colourSettings.planetMaterial), filePath + ".mat");
            // PrefabUtility.SaveAsPrefabAsset(gameObj, filePath + ".prefab", out bool success);
            // System.IO.File.WriteAllBytes(filePath + ".png", bytes);
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor);
    }
    
    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if(settings == null) 
        {
            return;
        }

        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if(!foldout)
            {
                return;
            } 

            CreateCachedEditor(settings, null, ref editor);
            editor.OnInspectorGUI();    

            if(check.changed)
            {
                onSettingsUpdated?.Invoke();
            } 
        }

    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }

    private void Save(int textureLength, int textureHeight, Texture2D texture, Material material)
    {
        RenderTexture buffer = new RenderTexture(
                               textureLength, 
                               textureHeight, 
                               0,                            // No depth/stencil buffer
                               RenderTextureFormat.ARGB32,   // Standard colour format
                               RenderTextureReadWrite.sRGB // No sRGB conversions
                           );

        Graphics.Blit(null, buffer, material);
        RenderTexture.active = buffer;           // If not using a scene camera

        texture.ReadPixels(
                    new Rect(0, 0, textureLength, textureHeight), // Capture the whole texture
                    0, 0,                                         // Write starting at the top-left texel
                    false);                                       // No mipmaps

        System.IO.File.WriteAllBytes("Assets/PlanetTexture.png", texture.EncodeToPNG());
    }
}
#endif