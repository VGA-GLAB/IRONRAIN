using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DescriptionAttribute))]
public sealed class DescriptionAttributeDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        var description = attribute as DescriptionAttribute;

        if (description is null)
        {
            return;
        }

        position.height = EditorGUIUtility.singleLineHeight * 2.5F;

        EditorGUI.HelpBox(position, description.Description, MessageType.Info);
    }

    public override float GetHeight() => EditorGUIUtility.singleLineHeight * 3F;
}