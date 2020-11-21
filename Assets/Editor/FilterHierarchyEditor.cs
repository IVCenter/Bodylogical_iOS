using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

enum FilterOptions {
    Tag,
    Layer,
    Both
}

public class FilterHierarchyEditor : EditorWindow {
    private FilterOptions filterOptions = FilterOptions.Both;
    private string selectedTag = "Untagged";
    private int layer;
    private bool filterInactive;
    private List<int> objectIndices;

    [MenuItem("Custom/Filter Hierarchy")]
    private static void Init() {
        FilterHierarchyEditor filter = EditorWindow.GetWindow(typeof (FilterHierarchyEditor)) as FilterHierarchyEditor;
        filter.Show();
    }

    private void OnGUI() {
        EditorGUILayout.PrefixLabel("Filtering Options");
        filterInactive = EditorGUILayout.Toggle("Filter Inactive", filterInactive);
        filterOptions = (FilterOptions) EditorGUILayout.EnumPopup("Filter By", filterOptions);

        if (filterOptions == FilterOptions.Tag) {
            selectedTag = EditorGUILayout.TagField("Select Tag", selectedTag);
            if (GUILayout.Button("Filter by Tag")) {
                FilterSelected();
            }
        } else if (filterOptions == FilterOptions.Layer) {
            layer = EditorGUILayout.LayerField("Select Layer", layer);
            if (GUILayout.Button("Filter by Layer")) {
                FilterSelected();
            }
        } else {
            selectedTag = EditorGUILayout.TagField("Select Tag", selectedTag);
            layer = EditorGUILayout.LayerField("Select Layer", layer);
            if (GUILayout.Button("Filter by All")) {
                FilterSelected();
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (Selection.objects.Length >= 1) {
            if (GUILayout.Button("Save Selection")) {
                int[] selectionIDs = Selection.instanceIDs;
                string saveStr = string.Empty;
                foreach (int i in selectionIDs) {
                    saveStr += i.ToString() + ";";
                }

                saveStr = saveStr.TrimEnd(char.Parse(";"));
                EditorPrefs.SetString("SelectedIDs", saveStr);
            }
        }

        if (EditorPrefs.HasKey("SelectedIDs")) {
            if (GUILayout.Button("Load Selection")) {
                string[] strIDs = EditorPrefs.GetString("SelectedIDs").Split(char.Parse(";"));
                int[] ids = new int[strIDs.Length];
                for (int i = 0; i < strIDs.Length; i++) {
                    ids[i] = int.Parse(strIDs[i]);
                }

                Selection.instanceIDs = ids;
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void OnInspectorUpdate() {
        Repaint();
    }

    private GameObject[] SelectAll() {
        return Resources.FindObjectsOfTypeAll<GameObject>();
    }

    private GameObject[] SelectActive() {
        return FindObjectsOfType<GameObject>();
    }

    private void FilterSelected() {
        GameObject[] selected = filterInactive ? SelectAll() : SelectActive();
        objectIndices = new List<int>();
        for (int i = 0; i < selected.Length; i++) {
            GameObject g = selected[i];
            if (filterOptions == FilterOptions.Tag && g.tag == selectedTag
                || filterOptions == FilterOptions.Layer && g.layer == layer
                || filterOptions == FilterOptions.Both && g.tag == selectedTag && g.layer == layer) {
                objectIndices.Add(i);
            }
        }
        
        Object[] newSelected = new Object[objectIndices.Count];
        for (int i = 0; i < objectIndices.Count; i++) {
            newSelected[i] = selected[objectIndices[i]];
        }

        Selection.objects = newSelected;
    }
}