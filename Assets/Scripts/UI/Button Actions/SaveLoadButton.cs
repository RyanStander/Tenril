
using UnityEngine;

public class SaveLoadButton : MonoBehaviour
{
    public void SaveGame()
    {
        EventManager.currentManager.AddEvent(new SaveData());
    }

    public void LoadGame()
    {
        EventManager.currentManager.AddEvent(new LoadData());
    }
}
