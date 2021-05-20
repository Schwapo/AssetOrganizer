using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
public class WatchedFolder
{
    [GUIColor("HighlightPathValidity")]
    [FolderPath(UseBackslashes = true)]
    [OnValueChanged("DisableSubdirectoriesIfNecessary")]
    public string path;

    [DisableIf("@path == \"Assets\"")]
    public bool includeSubdirectories;

    private Color HighlightPathValidity() => AssetDatabase.IsValidFolder(path)
        ? AssetOrganizerColors.Success
        : AssetOrganizerColors.Error;

    private void DisableSubdirectoriesIfNecessary()
    {
        if (path == "Assets")
            includeSubdirectories = false;
    }
}