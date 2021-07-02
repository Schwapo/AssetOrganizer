using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AssetOrganizerWindow : OdinEditorWindow
{
    public enum Tab
    {
        Organization,
        Help
    }

    [HideLabel]
    [EnumToggleButtons]
    [PropertySpace(0f, 12f)]
    public Tab CurrentTab;

    [PropertySpace(0f, 11f)]
    [ShowIf("@CurrentTab == Tab.Organization")]
    [TableList(NumberOfItemsPerPage = 10, ShowPaging = true)]
    public List<WatchedFolder> WatchedFolders = new List<WatchedFolder>();

    [ShowIf("@CurrentTab == Tab.Organization")]
    [TableList(NumberOfItemsPerPage = 10, ShowPaging = true)]
    public List<AssetOrganizationConfig> AssetOrganizationConfigs = new List<AssetOrganizationConfig>();

    public void Organize()
    {
        foreach (var folder in WatchedFolders)
        {
            var searchOption = folder.IncludeSubdirectories
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            var filePaths = Directory.GetFiles(AbsolutePath(folder.Path), "*", searchOption)
                .Where(path => Path.GetExtension(path) != ".meta")
                .Select(RelativePath);

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileExtension = Path.GetExtension(filePath);

                foreach (var assetOrganizationConfig in AssetOrganizationConfigs)
                {
                    if (assetOrganizationConfig.FilterString.IsNullOrWhitespace()
                        || assetOrganizationConfig.DestinationFolder.IsNullOrWhitespace())
                    {
                        return;
                    }

                    switch (assetOrganizationConfig.Filter)
                    {
                        case AssetOrganizationConfig.FilterType.Prefix:
                            if (fileName.StartsWith(assetOrganizationConfig.FilterString))
                            {
                                MoveAsset(filePath, assetOrganizationConfig.DestinationFolder);
                            }
                            break;

                        case AssetOrganizationConfig.FilterType.Suffix:
                            if (fileName.EndsWith(assetOrganizationConfig.FilterString))
                            {
                                MoveAsset(filePath, assetOrganizationConfig.DestinationFolder);
                            }
                            break;

                        case AssetOrganizationConfig.FilterType.Extension:
                            if (fileExtension.ToLower() == $".{assetOrganizationConfig.FilterString.ToLower()}")
                            {
                                MoveAsset(filePath, assetOrganizationConfig.DestinationFolder);
                            }
                            break;

                        case AssetOrganizationConfig.FilterType.Regex:
                            if (Regex.Match($"{fileName}{fileExtension}", assetOrganizationConfig.FilterString).Success)
                            {
                                MoveAsset(filePath, assetOrganizationConfig.DestinationFolder);
                            }
                            break;
                    }
                }
            }
        }
    }

    protected override void OnEnable()
    {
        var json = EditorPrefs.GetString(nameof(AssetOrganizerWindow));
        JsonUtility.FromJsonOverwrite(json, this);
    }

    private void OnDisable()
    {
        var json = JsonUtility.ToJson(this);
        EditorPrefs.SetString(nameof(AssetOrganizerWindow), json);
    }

    protected override void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(9);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(9);

        base.OnGUI();

        if (CurrentTab == Tab.Organization)
        {
            if (GUILayout.Button("Organize"))
            {
                Organize();
            }
        }

        GUILayout.Space(9);
        EditorGUILayout.EndVertical();
        GUILayout.Space(9);
        EditorGUILayout.EndHorizontal();
    }

    [OnInspectorGUI]
    [ShowIf("@CurrentTab == Tab.Help")]
    [FoldoutGroup("Watched folders")]
    [InfoBox("Under watched folders you will find all the folders that are taken into account when organizing your project so that you can ensure that you do not accidentally move files. Without this restriction, you could, for example, move files from other assets or important unity folders that should not be moved.", InfoMessageType.None)]
    public void WatchedFoldersInfo() { }

    [OnInspectorGUI]
    [ShowIf("@CurrentTab == Tab.Help")]
    [FoldoutGroup("Asset organization configs")]
    [InfoBox("Under Asset Organization Configs you can set the folder to which your assets should be moved. You can choose from the filter types Prefix, Suffix, Extension and Regular expression. Prefix, suffix and extension are relatively self-explanatory. Regex, on the other hand, can be very complex but also allows for more complex conditions. If you are not yet familiar with it, you should visit a reference website or use the buttons below.", InfoMessageType.None)]
    public void AssetOrganizationConfigsInfo() { }

    [Button]
    [PropertySpace(10f, 0f)]
    [ShowIf("@CurrentTab == Tab.Help")]
    [FoldoutGroup("Asset organization configs")]
    private void TestRegex() => Application.OpenURL("http://regexstorm.net/tester");

    [Button]
    [ShowIf("@CurrentTab == Tab.Help")]
    [FoldoutGroup("Asset organization configs")]
    private void RegexReference() => Application.OpenURL("http://regexstorm.net/reference");

    [OnInspectorGUI]
    [ShowIf("@CurrentTab == Tab.Help")]
    [FoldoutGroup("Color Meaning")]
    [InfoBox("Green means the filter string or path is a valid path that exists.\nYellow means that the path does not exist but will be created if needed.\nRed means the filter string or path is invalid.", InfoMessageType.None)]
    public void ColorMeaningInfo() { }

    private static string AbsolutePath(string relativePath)
        => Path.Combine(Path.GetDirectoryName(Application.dataPath), relativePath);

    private static string RelativePath(string absolutePath)
        => "Assets" + absolutePath.Substring(Application.dataPath.Length);

    private static void MoveAsset(string oldPath, string newPath)
    {
        if (!AssetDatabase.IsValidFolder(newPath))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(newPath), Path.GetFileName(newPath));
        }

        AssetDatabase.MoveAsset(oldPath, Path.Combine(newPath, Path.GetFileName(oldPath)));
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Schwapo/Asset Organizer")]
    private static void Open() => GetWindow<AssetOrganizerWindow>("Asset Organizer");
}
