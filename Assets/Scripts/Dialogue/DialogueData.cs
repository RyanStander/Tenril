using UnityEngine;

[CreateAssetMenu(menuName = "DialogueData")]
public class DialogueData : ScriptableObject
{
    [Tooltip("The name of the npc")] public string npcName;

    [Tooltip("The key for dialogue data to be pulled in order")]
    public string[] stringKeys;

    [Tooltip("The Words displayed for the following dialogue options")]
    public string[] options;

    [Tooltip("The dialogue data to load when the option is selected")]
    public DialogueData[] nextDialogue;

    [Header("Optional")] [Tooltip("Create 1 per character, this should be synced up to string keys")]
    public DialogueExtras[] dialogueExtras;

    [Tooltip(
        "This is linked to the string keys in order, if the 2nd key has no sound, make sure to make it empty and the 3rd to then have a sound if it should")]
    public AudioClip[] audioClipsToPlay;
}

[System.Serializable]
public class DialogueExtras
{
    public string name;
    [Tooltip(
        "This is linked to the string keys in order, if the 2nd key has no animation, make sure to make it empty and the 3rd to then have an animation if it should")]
    public AnimationClip[] animationsToPlay;
}