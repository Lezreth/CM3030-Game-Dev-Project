using UnityEngine;
using UnityEditor;

public class ShaderTesterWindow : EditorWindow
{
    private string _variableName = "_Saturation";
    private float _sliderValue = 0.0f;

    [MenuItem("Window/Shader Global Tester")]
    public static void ShowWindow()
    {
        GetWindow<ShaderTesterWindow>("Shader Tester");
    }

    void OnGUI()
    {
        GUILayout.Label("Global Shader Settings", EditorStyles.boldLabel);

        _variableName = EditorGUILayout.TextField("Shader Variable Name", _variableName);

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        _sliderValue = EditorGUILayout.Slider("Effect Value", _sliderValue, 0.0f, 1.0f);

        if (EditorGUI.EndChangeCheck())
        {
            UpdateShader();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Force Global Refresh"))
        {
            UpdateShader();
        }

        EditorGUILayout.HelpBox($"Currently setting {_variableName} to: {_sliderValue:F2}", MessageType.Info);
    }

    void UpdateShader()
    {
        if (string.IsNullOrEmpty(_variableName)) return;

        // Set the global value using the float
        Shader.SetGlobalFloat(_variableName, _sliderValue);

        SceneView.RepaintAll();
        Debug.Log($"[ShaderTester] {_variableName} set to {_sliderValue}");
    }
}
