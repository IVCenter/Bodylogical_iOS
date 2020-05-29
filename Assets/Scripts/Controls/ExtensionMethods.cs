using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    /// <summary>
    /// Depth-First Search through all children and return an list of children with the specified name.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <param name="partial">If set to false, an exact match will be checked.</param>
    public static List<GameObject> SearchAllWithName(this Transform target, string name, bool partial = false) {
        List<GameObject> results = new List<GameObject>();

        if (target.name == name || (partial && target.name.Contains(name))) {
            results.Add(target.gameObject);
        }

        foreach (Transform t in target)
            results.AddRange(t.SearchAllWithName(name, partial));

        return results;
    }

    /// <summary>
    /// Depth-First Search through all the children to find the first children that matches the name
    /// </summary>
    public static Transform Search(this Transform target, string name) {
        if (target.name == name) return target;

        for (int i = 0; i < target.childCount; ++i) {
            var result = Search(target.GetChild(i), name);

            if (result != null) return result;

        }

        return null;
    }

    /// <summary>
    /// Depth-First Search through all children and return a list of children with the specified components.
    /// </summary>
    /// <typeparam name="T">Component toe be found.</typeparam>
    public static List<T> SearchAllWithType<T>(this Transform target) where T : Component {
        List<T> results = new List<T>();

        T component = target.GetComponent<T>();
        if (component != null) {
            results.Add(component);
        }

        foreach(Transform t in target) {
            results.AddRange(t.SearchAllWithType<T>());
        }

        return results;
    }
}
