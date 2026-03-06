using UnityEngine;
using UnityEditor;

public class ShaderTesterWindow : EditorWindow
{
    private string _variableName = "_Saturation"; // Default name
    private bool _isEffectEnabled;

    [MenuItem("Window/Shader Global Tester")]
    public static void ShowWindow()
    {
        GetWindow<ShaderTesterWindow>("Shader Tester");
    }

    void OnGUI()
    {
        GUILayout.Label("Global Shader Settings", EditorStyles.boldLabel);

        // 1. Text Box for Variable Name
        _variableName = EditorGUILayout.TextField("Shader Variable Name", _variableName);

        EditorGUILayout.Space();

        // 2. Toggle Switch
        EditorGUI.BeginChangeCheck();
        _isEffectEnabled = EditorGUILayout.Toggle("Enable Effect (1.0 / 0.0)", _isEffectEnabled);

        if (EditorGUI.EndChangeCheck())
        {
            UpdateShader();
        }

        EditorGUILayout.Space();

        // 3. Debug Help
        if (GUILayout.Button("Force Global Refresh"))
        {
            UpdateShader();
        }

        EditorGUILayout.HelpBox($"Currently setting {_variableName} to: {(_isEffectEnabled ? "1.0" : "0.0")}", MessageType.Info);
    }

    void UpdateShader()
    {
        if (string.IsNullOrEmpty(_variableName)) return;

        // Set the global value
        Shader.SetGlobalFloat(_variableName, _isEffectEnabled ? 1.0f : 0.0f);

        // Force the Scene View to update immediately
        SceneView.RepaintAll();
        Debug.Log($"[ShaderTester] {_variableName} set to {(_isEffectEnabled ? "1.0" : "0.0")}");
    }
}
