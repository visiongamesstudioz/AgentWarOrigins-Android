

using System;
using UnityEditor;
using UnityEngine;

public static class MB_EditorUtil
{
    private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
    private static GUIContent moveButtonContent = new GUIContent("↴", "move down");
    private static GUIContent duplicateButtonContent = new GUIContent("+", "duplicate");
    private static GUIContent deleteButtonContent = new GUIContent("-", "delete");
    private static GUIContent addButtonContent = new GUIContent("+", "add element");

    public static void DrawSeparator()
    {
        GUILayout.Space(12f);
        if (Event.current.type != (EventType) 7)
            return;
        Texture2D whiteTexture = EditorGUIUtility.whiteTexture;
        Rect lastRect = GUILayoutUtility.GetLastRect();
        GUI.color=(new Color(0.0f, 0.0f, 0.0f, 0.25f));
        // ISSUE: explicit reference operation
        GUI.DrawTexture(new Rect(0.0f, (lastRect).yMin + 6f, (float)Screen.width, 4f), (Texture)whiteTexture);
        // ISSUE: explicit reference operation
        GUI.DrawTexture(new Rect(0.0f, (lastRect).yMin + 6f, (float)Screen.width, 1f), (Texture)whiteTexture);
        // ISSUE: explicit reference operation
        GUI.DrawTexture(new Rect(0.0f, (lastRect).yMin + 9f, (float)Screen.width, 1f), (Texture)whiteTexture);
        GUI.color=(Color.white);
    }

    public static Rect DrawHeader(string text)
    {
        GUILayout.Space(28f);
        Rect rect = GUILayoutUtility.GetLastRect();
        rect.yMin += 5f;
        rect.yMax -= 4f;
        rect.width = Screen.width;

        if (Event.current.type == EventType.Repaint)
        {
            GUI.color = Color.black;
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin, Screen.width, 1f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(0f, rect.yMax - 1, Screen.width, 1f), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 4f, rect.y, rect.width - 4, rect.height), text, EditorStyles.boldLabel);
        }
        return rect;
    }

    public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default)
    {
        if (!list.isArray)
        {
            EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", (MessageType)3);
        }
        else
        {
            bool flag1 = (uint)(options & EditorListOption.ListLabel) > 0U;
            bool flag2 = (uint)(options & EditorListOption.ListSize) > 0U;
            if (flag1)
            {
                EditorGUILayout.PropertyField(list, new GUILayoutOption[0]);
                EditorGUI.indentLevel=(EditorGUI.indentLevel + 1);
            }
            if (!flag1 || list.isExpanded)
            {
                SerializedProperty propertyRelative = list.FindPropertyRelative("Array.size");
                if (flag2)
                    EditorGUILayout.PropertyField(propertyRelative, new GUILayoutOption[0]);
                if (propertyRelative.hasMultipleDifferentValues)
                    EditorGUILayout.HelpBox("Not showing lists with different sizes.", (MessageType)1);
                else
                    ShowElements(list, options);
            }
            if (!flag1)
                return;
            EditorGUI.indentLevel=(EditorGUI.indentLevel - 1);
        }
    }

    private static void ShowElements(SerializedProperty list, EditorListOption options)
    {
        bool flag1 = (uint)(options & EditorListOption.ElementLabels) > 0U;
        bool flag2 = (uint)(options & EditorListOption.Buttons) > 0U;
        for (int index = 0; index < list.arraySize; ++index)
        {
            if (flag2)
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (flag1)
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(index), new GUILayoutOption[0]);
            else
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(index), (GUIContent)GUIContent.none, new GUILayoutOption[0]);
            if (flag2)
            {
                ShowButtons(list, index);
                EditorGUILayout.EndHorizontal();
            }
        }
        if (!flag2 || list.arraySize != 0 || !GUILayout.Button(addButtonContent, EditorStyles.miniButton, new GUILayoutOption[0]))
            return;
        SerializedProperty serializedProperty = list;
        serializedProperty.arraySize=(serializedProperty.arraySize + 1);
    }

    private static void ShowButtons(SerializedProperty list, int index)
    {
        if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, new GUILayoutOption[1]
        {
      miniButtonWidth
        }))
            list.MoveArrayElement(index, index + 1);
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, new GUILayoutOption[1]
        {
      miniButtonWidth
        }))
            list.InsertArrayElementAtIndex(index);
        if (!GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, new GUILayoutOption[1]
        {
      miniButtonWidth
        }))
            return;
        int arraySize = list.arraySize;
        list.DeleteArrayElementAtIndex(index);
        if (list.arraySize == arraySize)
            list.DeleteArrayElementAtIndex(index);
    }

    [Flags]
    public enum EditorListOption
    {
        None = 0,
        ListSize = 1,
        ListLabel = 2,
        ElementLabels = 4,
        Buttons = 8,
        Default = ElementLabels | ListLabel | ListSize, // 0x00000007
        NoElementLabels = ListLabel | ListSize, // 0x00000003
        All = NoElementLabels | Buttons | ElementLabels, // 0x0000000F
    }
}
