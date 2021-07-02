using UnityEditor;
using UnityEngine;

public static class AssetOrganizerColors
{
    public static Color Success => EditorGUIUtility.isProSkin
        ? new Color(0.5f, 1f, 0.5f)
        : new Color(0.78f, 1f, 0.78f);

    public static Color Caution => EditorGUIUtility.isProSkin
        ? new Color(1f, 1f, 0.2f)
        : new Color(1f, 1f, 0.78f);

    public static Color Error => EditorGUIUtility.isProSkin
        ? new Color(1f, 0.5f, 0.5f)
        : new Color(1f, 0.78f, 0.78f);
}
