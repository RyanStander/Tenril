using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueData")]
public class DialogueData : ScriptableObject
{
    [Tooltip("The name of the npc")]
    public string npcName;
    [Tooltip("The key for dialogue data to be pulled in order")]
    public string[] stringKeys;

    [Tooltip("The Words displayed for the following dialogue options")]
    public string[] options;
    [Tooltip("The dialogue data to load when the option is selected")]
    public DialogueData[] nextDialogue;
}
