using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class OpenScriptButtonAttribute : PropertyAttribute
{
    public string ScriptPath { get; }

    public OpenScriptButtonAttribute(Type SequenceType)
    {
        ScriptPath = $"Assets/Ingame/Script/Sequence System/Sequence/{SequenceType.Name}.cs";
    }

    public OpenScriptButtonAttribute(string openScriptPath) => ScriptPath = openScriptPath;
}