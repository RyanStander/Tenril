using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Editor designed to allow for full customization of a loot table from the inspector without needing external objects to be added
/// Editor was designed using the following source for assisntance - https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
/// </summary>

[CustomEditor(typeof(LootTable))]
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
			int numberOfProperties = 3; //A quick way of making changes to the editor below
			float editorWidth = rect.width - (propertyPadding * (numberOfProperties - 1)); //Usable space after removing padding
			float propertyWidth = editorWidth / numberOfProperties;

			//Find the fields of the list being checked and link it as a GUI property field
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("definedItem"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + propertyWidth + propertyPadding, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("itemCountRange"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + (2 * propertyWidth) + (2 * propertyPadding), rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("spawnChance"), GUIContent.none);
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
