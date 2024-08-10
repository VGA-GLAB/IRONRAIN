using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OpenScriptButtonAttribute))]
public class OpenScriptButtonAttributeDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        position.height = EditorGUIUtility.singleLineHeight;

        if (GUI.Button(position, "Open Script") && attribute is OpenScriptButtonAttribute attr)
        {
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(
                attr.ScriptPath);

            AssetDatabase.OpenAsset(script);
        }
    }

    public override float GetHeight() => EditorGUIUtility.singleLineHeight * 1.5F;
}