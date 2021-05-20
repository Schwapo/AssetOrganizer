using System;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AssetOrganizationConfig
{
    public enum FilterType
    {
        Prefix,
        Suffix,
        Extension,
        Regex
    }
    
    [TableColumnWidth(100, Resizable = false)]
    public FilterType filterType;

    [GUIColor("HighlightFilterStringValidity")]
    public string filterString;

    [FolderPath(UseBackslashes = true)]
    [GUIColor("HighlightDestinationFolderValidity")]
    public string destinationFolder;

    private Color HighlightFilterStringValidity()
    {
        if (filterString.IsNullOrWhitespace()) 
            return AssetOrganizerColors.Error;
        
        switch (filterType)
        {
            case FilterType.Prefix:
            case FilterType.Suffix:
                return AssetOrganizerColors.Success;
            
            case FilterType.Extension:
                return filterString.StartsWith(".")
                    ? AssetOrganizerColors.Success
                    : AssetOrganizerColors.Error;
            
            case FilterType.Regex:
                return IsValidRegex(filterString)
                    ? AssetOrganizerColors.Success
                    : AssetOrganizerColors.Error;
            
            default: 
                return AssetOrganizerColors.Success;
        }
    }

    private Color HighlightDestinationFolderValidity()
    {
        if (destinationFolder.IsNullOrWhitespace())
            return AssetOrganizerColors.Error;

        return AssetDatabase.IsValidFolder(destinationFolder)
            ? AssetOrganizerColors.Success
            : AssetOrganizerColors.Caution;
    }

    private static bool IsValidRegex(string pattern)
    {
        try { Regex.Match("", pattern); }
        catch (ArgumentException) { return false; }
        return true;
    }
}