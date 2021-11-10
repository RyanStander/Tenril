using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("The name of the speaking character")]
    public string name;
    [Tooltip("The dialogue the the npc will have")]
    [TextArea(3,10)]public string[] sentences;

    //following option
    public string optionName;

    //some identifier to indicate what the next possible dialogues are
}
