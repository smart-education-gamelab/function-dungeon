using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Utils {
    public static bool Within(int value, int min, int max) {
        return value >= min && value < max;
    }

    public static Sequence DelayedAction(float delay, System.Action action) {
        return DOTween.Sequence().AppendInterval(delay).AppendCallback(() => action());
    }

    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveInTargetLocalSpace(Transform transform, Transform target, Vector3 targetLocalEndPosition, float duration) {
        var t = DOTween.To(
            () => transform.position - target.transform.position,
            x => transform.position = x + target.transform.position,
            targetLocalEndPosition,
            duration);
        t.SetTarget(transform);
        return t;
    }

#if UNITY_EDITOR
    public static void GuiLine(int i_height = 1) {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    //Different kind of textfield that does not select text on focus.
    public static void DeselectOnFocusTextField(SerializedProperty stringProperty, GUIContent label, params GUILayoutOption[] options) {
        // Generate a unique ID for the text field using the property path
        string textFieldID = "TextField_" + stringProperty.propertyPath;
        GUI.SetNextControlName(textFieldID);

        // Draw the label and text field
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        EditorGUI.BeginChangeCheck();

        // Create a GUIStyle that mimics a TextField to get the correct padding and margins
        GUIStyle textFieldStyle = GUI.skin.textField;

        // Store the current value of the text field
        string textValue = stringProperty.stringValue;

        // Draw the text field and get the latest input from the user
        int textFieldIDHash = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
        string newTextValue = GUILayout.TextField(textValue, textFieldStyle, options);



        // Get the state of the text editor associated with the current control.
        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), textFieldIDHash);

        // Handle the keyboard input for Ctrl+A to select all text.
        if (editor != null) {
            Event currentEvent = Event.current;
            bool isControlPressed = currentEvent.control || currentEvent.command; // command is used for macOS (Ctrl key equivalent)
            if (isControlPressed && currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.A) {
                editor.SelectAll();
                currentEvent.Use(); // Mark the event as used, so it doesn't propagate further.
            }
        }

        if (EditorGUI.EndChangeCheck()) {
            stringProperty.stringValue = newTextValue;
        }

        EditorGUILayout.EndHorizontal();

        // Apply the changes to the serialized object
        stringProperty.serializedObject.ApplyModifiedProperties();

        // Now handle deselecting the text when the control receives focus
        if (GUIUtility.keyboardControl == textFieldIDHash) {
            // If this is the first time we're clicking on this text field
            if (Event.current.type == EventType.MouseDown) {
                // Focus the field and deselect all text
                GUI.FocusControl(textFieldID);
                if (editor != null) {
                    editor.selectIndex = editor.cursorIndex;
                }
            }
        }
    }

    //Different kind of textarea that does not select text on focus.
    public static void DeselectOnFocusTextArea(SerializedProperty stringProperty, GUIContent label, params GUILayoutOption[] options) {
        // Generate a unique ID for the text field using the property path
        string textFieldID = "TextArea_" + stringProperty.propertyPath;
        GUI.SetNextControlName(textFieldID);

        // Draw the label and text field
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        EditorGUI.BeginChangeCheck();

        // Create a GUIStyle that mimics a TextField to get the correct padding and margins
        GUIStyle textFieldStyle = GUI.skin.textField;
        textFieldStyle.wordWrap = true;

        // Store the current value of the text field
        string textValue = stringProperty.stringValue;

        // Draw the text field and get the latest input from the user
        int textAreaIDHash = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
        string newTextValue = GUILayout.TextArea(textValue, textFieldStyle, options);

        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), textAreaIDHash);
        if (EditorGUI.EndChangeCheck()) {
            stringProperty.stringValue = newTextValue;
        }

        EditorGUILayout.EndHorizontal();

        // Apply the changes to the serialized object
        stringProperty.serializedObject.ApplyModifiedProperties();

        // Now handle deselecting the text when the control receives focus
        if (GUIUtility.keyboardControl == textAreaIDHash) {
            // If this is the first time we're clicking on this text field
            if (Event.current.type == EventType.MouseDown) {
                // Focus the field and deselect all text
                GUI.FocusControl(textFieldID);
                if (editor != null) {
                    editor.selectIndex = editor.cursorIndex;
                }
            }
        }
    }

    public static float RangeSliderWithLabels(GUIContent label, string minLabel, string maxLabel, float value, float min, float max) {
        const float extraLabelSpacing = 10;
        const float minSliderSize = 50;
        var minLabelWidth = GUI.skin.label.CalcSize(new GUIContent(minLabel)).x + extraLabelSpacing;
        var maxLabelWidth = GUI.skin.label.CalcSize(new GUIContent(maxLabel)).x + extraLabelSpacing;

        Rect controlRect = EditorGUILayout.GetControlRect();
        Rect labelRect = new Rect(controlRect.position.x, controlRect.position.y, EditorGUIUtility.labelWidth, controlRect.height);
        Rect contentRect = new Rect(controlRect.position.x + EditorGUIUtility.labelWidth, controlRect.position.y, controlRect.width - EditorGUIUtility.labelWidth, controlRect.height);

        bool textFits = contentRect.width > minLabelWidth + maxLabelWidth + minSliderSize;
        float minLabelFinalWidth = textFits ? minLabelWidth : 0;
        float maxLabelFinalWidth = textFits ? maxLabelWidth : 0;
        float sliderWidth = textFits ? contentRect.width - minLabelWidth - maxLabelWidth : contentRect.width;
        Rect minLabelRect = new Rect(contentRect.x, contentRect.y, minLabelFinalWidth, controlRect.height);
        Rect maxLabelRect = new Rect(controlRect.width - maxLabelWidth, contentRect.y, maxLabelFinalWidth, controlRect.height);

        float sliderX = textFits ? minLabelRect.x + minLabelWidth : contentRect.x;
        Rect sliderRect = new Rect(sliderX, contentRect.y, sliderWidth, controlRect.height);

        GUI.Label(labelRect, label);
        GUI.Label(minLabelRect, minLabel);
        GUIStyle rightAlignedStyle = new GUIStyle(GUI.skin.label);
        rightAlignedStyle.alignment = TextAnchor.MiddleRight;
        GUI.Label(maxLabelRect, maxLabel, rightAlignedStyle);
        return GUI.HorizontalSlider(sliderRect, value, min, max);
    }
#endif

    public static string Base64Encode(string input) {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        return System.Convert.ToBase64String(inputBytes);
    }

    public static string Base64Decode(string input) {
        var inputBytes = System.Convert.FromBase64String(input);
        return System.Text.Encoding.ASCII.GetString(inputBytes);
    }

    public static Dictionary<T, int> CountOccurrences<T>(List<T> sections) {
        var sectionCounts = new Dictionary<T, int>();

        foreach (var section in sections) {
            if (sectionCounts.ContainsKey(section)) {
                sectionCounts[section]++;
            } else {
                sectionCounts[section] = 1;
            }
        }

        return sectionCounts;
    }

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
