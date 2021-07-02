using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Text.RegularExpressions;
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
    public FilterType Filter;

    [ValueColor(nameof(FilterStatusColor))]
    public string FilterString;

    [FolderPath(UseBackslashes = true)]
    [ValueColor(nameof(DestinationFolderStatusColor))]
    public string DestinationFolder;

    private Color FilterStatusColor()
    {
        if (FilterString.IsNullOrWhitespace())
        {
            return AssetOrganizerColors.Error;
        }

        switch (Filter)
        {
            case FilterType.Prefix:
            case FilterType.Suffix:
                return AssetOrganizerColors.Success;

            case FilterType.Extension:
                return FilterString.StartsWith(".")
                    ? AssetOrganizerColors.Error
                    : AssetOrganizerColors.Success;

            case FilterType.Regex:
                return IsValidRegex(FilterString)
                    ? AssetOrganizerColors.Success
                    : AssetOrganizerColors.Error;

            default:
                return AssetOrganizerColors.Success;
        }
    }

    private Color DestinationFolderStatusColor()
    {
        if (DestinationFolder.IsNullOrWhitespace() || !DestinationFolder.StartsWith("Assets"))
        {
            return AssetOrganizerColors.Error;
        }

        return AssetDatabase.IsValidFolder(DestinationFolder)
            ? AssetOrganizerColors.Success
            : AssetOrganizerColors.Caution;
    }

    private static bool IsValidRegex(string pattern)
    {
        try
        {
            Regex.Match("", pattern);
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }
}
