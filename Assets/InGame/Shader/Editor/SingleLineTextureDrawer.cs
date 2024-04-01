using UnityEditor;
using UnityEngine;

public class SingleLineTextureDrawer : MaterialPropertyDrawer
{
    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        var value = editor.TexturePropertySingleLine(label, prop);
    }
}
