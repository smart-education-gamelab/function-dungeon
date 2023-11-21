using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils { 
    /*Find instances of objects. Mostly used in Globals*/
    #region ObjectFinding
    private static T CheckIfUniqueObject<T>(T[] collection, string name, bool errorWhenNoneFound = true) where T : class {
        if (errorWhenNoneFound && collection.Length == 0) Debug.LogError($"No instances of unique type {name} could be found. This will probably break a lot of stuff!");
        if (collection.Length == 1) return collection[0];
        if (collection.Length > 1) Debug.LogError($"Multiple instances of unique type {name} could be found. This will probably break a lot of stuff!");
        return null;
    }

    public static bool FindUniqueObjectInChildren<T>(GameObject parent, out T obj, bool errorWhenNoneFound = true) where T : Object {
        obj = CheckIfUniqueObject(parent.GetComponentsInChildren<T>(true), typeof(T).Name, errorWhenNoneFound);
        return obj != null;
    }
    public static bool FindUniqueObject<T>(out T obj, bool errorWhenNoneFound = true) where T : Object {
        obj = CheckIfUniqueObject(GameObject.FindObjectsOfType<T>(true), typeof(T).Name, errorWhenNoneFound);
        return obj != null;
    }

    public static T FindUniqueObject<T>() where T : Object {
        return CheckIfUniqueObject(GameObject.FindObjectsOfType<T>(), typeof(T).Name);
    }

    public static GameObject FindUniqueGameObjectWithTag(string tag) {
        return CheckIfUniqueObject(GameObject.FindGameObjectsWithTag(tag), tag);
    }

    public static T FindUniqueObjectWithTag<T>(string tag) where T : Component {
        return FindUniqueGameObjectWithTag(tag)?.GetComponent<T>();
    }

    /*Logs an error when les or more than one instance of the type is found in the scene*/
    public static void EnsureOnlyOneInstanceInScene<T>() where T : Object {
        FindUniqueObject<T>();
    }
    #endregion
}
