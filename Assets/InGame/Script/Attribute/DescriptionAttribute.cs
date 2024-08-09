using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
public sealed class DescriptionAttribute : PropertyAttribute
{
    public string Description { get; private set; }

    public DescriptionAttribute(string description) => Description = description;
}