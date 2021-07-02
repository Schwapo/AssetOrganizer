using System;
using System.Diagnostics;
using UnityEngine;

[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public class ValueColorAttribute : Attribute
{
    public Color Color;
    public string GetColor;

    public ValueColorAttribute(string getColor) => GetColor = getColor;
    public ValueColorAttribute(float r, float g, float b, float a = 1f) => Color = new Color(r, g, b, a);
}