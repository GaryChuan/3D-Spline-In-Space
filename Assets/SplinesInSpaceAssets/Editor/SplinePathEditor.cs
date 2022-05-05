#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplinePath))]
public class SplinePathEditor : Editor
{
    SplinePath splinePath;
    
    Editor splineEditor;
    Editor splineColourEditor;

    public override void OnInspectorGUI()
    {
        using(var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if(check.changed)
            {
                splinePath.GenerateSpline();
            }
        }
            
        if(GUILayout.Button("Generate Spline"))
        {
            splinePath.GenerateSpline();
        }

        DrawSettingsEditor(
            splinePath.splineSettings, 
            splinePath.OnSplineSettingsUpdated, 
            ref splinePath.splineSettingsFoldout, 
            ref splineEditor);

        DrawSettingsEditor(
            splinePath.colourSettings, 
            splinePath.OnColourSettingsUpdated, 
            ref splinePath.colourSettingsFoldout, 
            ref splineColourEditor);
    }


    void DrawSettingsEditor(
        Object settings, System.Action 
        onSettingsUpdated, 
        ref bool foldout, 
        ref Editor editor)
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
        splinePath = (SplinePath)target;
    }
}

#endif