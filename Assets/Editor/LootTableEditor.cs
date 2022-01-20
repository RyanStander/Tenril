using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Editor designed to allow for full customization of a loot table from the inspector without needing scriptable objects to be added and to create an easy to use format
/// Editor was designed using the following sources for assisntance :
/// - https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
/// - https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
/// Currently this is disabled due to formatting issues and not being fully functional
/// </summary>

//CustomEditor(typeof(LootTable))]
public class LootTableEditor : Editor
{
    private ReorderableList miscellaneous;

	private void OnEnable()
	{
        //Set the list for miscellaneous loot
        miscellaneous = new ReorderableList(serializedObject, serializedObject.FindProperty("miscellaneousLoot"), true, true, true, true);

        //Apply the structure needed for the list
        miscellaneous.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            //Declare the list that should be serialized/structured
            var element = miscellaneous.serializedProperty.GetArrayElementAtIndex(index);

            float propertyPadding = 10;
            int numberOfPropertiesAndLabels = 5; //A quick way of making changes to the editor below
            float editorWidth = rect.width - (propertyPadding * (numberOfPropertiesAndLabels - 1)); //Usable space after removing padding
            float propertyWidth = editorWidth / numberOfPropertiesAndLabels;

            //Find the fields of the list being checked and link it as a GUI property field
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("definedItem"), GUIContent.none);
           
            EditorGUI.LabelField(new Rect(rect.x + propertyWidth + propertyPadding, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), "Count");
            EditorGUI.PropertyField(new Rect(rect.x + (2 * propertyWidth) + (2 * propertyPadding), rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("itemCountRange"), GUIContent.none);
            
            EditorGUI.LabelField(new Rect(rect.x + (3 * propertyWidth) + (3 * propertyPadding), rect.y, 100, EditorGUIUtility.singleLineHeight), "Chance");
            EditorGUI.PropertyField(new Rect(rect.x + (4 * propertyWidth) + (4 * propertyPadding), rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("spawnChance"), GUIContent.none);
        };
    }

	public override void OnInspectorGUI()
	{
		//Update
		serializedObject.Update();

		//Apply layout for each list
		EditorGUILayout.LabelField("Miscellaneous Loot");
		miscellaneous.DoLayoutList();

		//Apply properties
		serializedObject.ApplyModifiedProperties();
	}
}
